using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Practice_Project.Entities
{
    public class Student
    {
        [Key]
        public int StudentId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        public int Semester { get; set; } 
        
        [Phone]
        public string? Phone { get; set; } = string.Empty;
        
        public string? RollNumber { get; set; }
        
        // Authentication properties
        public string? LoginId { get; set; } // Generated student ID for login
        
        public string? Password { get; set; } // Hashed password
        
        public bool IsActive { get; set; } = true;
        
        // Navigation properties
        public virtual ICollection<BookIssue> BookIssues { get; set; } = new List<BookIssue>();
        
        // Method to generate login ID
        public void GenerateLoginId()
        {
            var year = DateTime.Now.Year.ToString().Substring(2);
            var random = new Random();
            var number = random.Next(1000, 9999);
            LoginId = $"LIB{year}{number}";
        }
    }
}