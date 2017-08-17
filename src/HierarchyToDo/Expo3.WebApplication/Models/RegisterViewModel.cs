using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Expo3.WebApplication.Models
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
