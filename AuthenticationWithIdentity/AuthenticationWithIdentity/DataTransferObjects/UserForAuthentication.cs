using System.ComponentModel.DataAnnotations;

namespace AuthenticationWithIdentity.DataTransferObjects
{
    public record UserForAuthentication
    {
        [Required]
        public string? UserName { get; set; }
        [Required]
        public string? Password { get; set; }
    }
}
