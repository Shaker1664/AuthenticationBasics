using AuthenticationWithIdentity.DataTransferObjects;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationWithIdentity.Interfaces
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> RegisterUser(UserForRegistration user);
        Task<bool> ValidateUser(UserForAuthentication user);
        Task<AuthenticationResponse> AuthToken(bool populateExp);
        Task<AuthenticationResponse> RefreshToken(AuthenticationResponse token);
    }
}
