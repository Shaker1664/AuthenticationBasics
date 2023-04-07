using System.ComponentModel.DataAnnotations;

namespace AuthenticationWithIdentity.DataTransferObjects
{
    public class UserForAuthentication
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
