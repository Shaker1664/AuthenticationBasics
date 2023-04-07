using AuthenticationWithIdentity.DataTransferObjects;
using AuthenticationWithIdentity.Entities;
using AuthenticationWithIdentity.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationWithIdentity.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IConfiguration _configuration;


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
    }
}
