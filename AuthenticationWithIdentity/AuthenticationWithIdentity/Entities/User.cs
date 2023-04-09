using Microsoft.AspNetCore.Identity;

namespace AuthenticationWithIdentity.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime ExpiryTime { get; set; }
    }
}
