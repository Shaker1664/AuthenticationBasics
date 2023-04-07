using AuthenticationWithIdentity.DataTransferObjects;
using AuthenticationWithIdentity.Entities;
using AuthenticationWithIdentity.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
        
        public async Task<string> AuthToken()
        {
            var signingCreds = GetSigningCredentials();
            var claims = await GetClaims();
            var token = CreateToken(signingCreds, claims);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

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


    }
}
