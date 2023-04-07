using AuthenticationWithIdentity.DataTransferObjects;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationWithIdentity.Interfaces
{
    public interface IAuthenticationService
    {
        Task<IdentityResult> RegisterUser(UserForRegistration user);
    }
}
