using AuthenticationWithIdentity.DataTransferObjects;
using AuthenticationWithIdentity.Entities;
using AuthenticationWithIdentity.Exceptions;
using AuthenticationWithIdentity.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthenticationWithIdentity.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IConfiguration _configuration;
        private User? _user;


        public AuthenticationService(UserManager<User> userManager, ILogger<AuthenticationService> logger, IConfiguration configuration, IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration;
            _mapper = mapper;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> RegisterUser(UserForRegistration userForRegistration)
        {
            var user = _mapper.Map<User>(userForRegistration);

            var result = await _userManager.CreateAsync(user, userForRegistration.Password);

            if(result.Succeeded)
            {
                await _userManager.AddToRolesAsync(user, userForRegistration.Roles);
            }

            return result;
        }

        public async Task<bool> ValidateUser(UserForAuthentication user)
        {
            _user = await _userManager.FindByNameAsync(user.UserName);

            var result = (_user != null && await _userManager.CheckPasswordAsync(_user, user.Password));
            if (!result)
                _logger.LogWarning($"{nameof(ValidateUser)}: Authentication failed");
            return result;
        }
        
        public async Task<AuthenticationResponse> AuthToken(bool populateExp)
        {
            var signingCreds = GetSigningCredentials();
            var claims = await GetClaims();
            var token = CreateToken(signingCreds, claims);
            var refreshToken = GenerateRefreshToken();

            _user.RefreshToken = refreshToken;

            if (populateExp)
            {
                _user.ExpiryTime = DateTime.UtcNow.AddHours(8);
            }

            await _userManager.UpdateAsync(_user);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthenticationResponse { AccessToken = accessToken, RefreshToken = refreshToken};
        }

        public async Task<AuthenticationResponse> RefreshToken(AuthenticationResponse token)
        {
            var principal = GetPrinciplalFromExpiredToken(token.AccessToken);

            var user = await _userManager.FindByNameAsync(principal.Identity.Name);

            if(user == null || user.RefreshToken != token.RefreshToken || user.ExpiryTime <= DateTime.UtcNow)
            {
                throw new RefreshTokenBadRequest();
            }
            return await AuthToken(true);
        }

        //private methods
        #region
        private SigningCredentials GetSigningCredentials()
        {
            var key = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWTSECRET"));
            var secret = new SymmetricSecurityKey(key);
            return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
        }
        private async Task<List<Claim>> GetClaims()
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, _user.UserName)
            };

            var roles = await _userManager.GetRolesAsync(_user);
            foreach(var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            return claims;
        }
        private JwtSecurityToken CreateToken(SigningCredentials signingCreds, List<Claim> claims)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var token = new JwtSecurityToken
            (
                issuer: jwtSettings["validIssuer"],
                audience: jwtSettings["validAudience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(jwtSettings["expires"])),
                signingCredentials: signingCreds
            );

            return token;
        }
        private string GenerateRefreshToken()
        {
            var rnd = new byte[32];
            using(var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(rnd);
                return Convert.ToBase64String(rnd);
            }
        }
        private ClaimsPrincipal GetPrinciplalFromExpiredToken(string token)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");

            var tokenValidtionParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWTSECRET"))),
                ValidateLifetime = true,
                ValidIssuer = jwtSettings["validIssuer"],
                ValidAudience = jwtSettings["validAudience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidtionParameters, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }
        #endregion
    }
}
