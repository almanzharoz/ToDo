using System.ComponentModel.DataAnnotations;

namespace Expo3.WebApplication.Models.Account
{
    public class RegisterViewModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password")]
        public string RetryPassword { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        public string Result { get; set; }
    }
}
