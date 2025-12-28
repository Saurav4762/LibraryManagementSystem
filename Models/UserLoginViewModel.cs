// Models/UserLoginViewModel.cs
using System.ComponentModel.DataAnnotations;

namespace Practice_Project.Models
{
    public class UserLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = false;
    }
}