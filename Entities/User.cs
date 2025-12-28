// Entities/User.cs
using System.ComponentModel.DataAnnotations;

namespace Practice_Project.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty; // Stores HASHED password only!

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "Librarian"; // "Admin" or "Librarian"

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}