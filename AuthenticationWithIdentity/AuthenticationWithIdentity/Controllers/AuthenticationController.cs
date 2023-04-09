using AuthenticationWithIdentity.DataTransferObjects;
using AuthenticationWithIdentity.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthenticationWithIdentity.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticate;

        public AuthenticationController(IAuthenticationService authenticate)
        {
            _authenticate = authenticate;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] UserForRegistration userForRegistration)
        {
            if (userForRegistration == null)
            {
                return BadRequest();
            }
            var result = await _authenticate.RegisterUser(userForRegistration);
            if(!result.Succeeded) 
            {
                foreach (var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Authenticate(UserForAuthentication user)
        {
            if (!await _authenticate.ValidateUser(user))
            {
                return Unauthorized();
            }
            return Ok(new { Token = await _authenticate.AuthToken(populateExp: true) });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody]AuthenticationResponse response)
        {
            var refreshToken = await _authenticate.RefreshToken(response);

            return Ok(refreshToken);
        }
    }
}
