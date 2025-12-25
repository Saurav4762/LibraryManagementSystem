using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Practice_Project.Models
{
    public class StudentVm
    {
        [Key]
        public int StudentId { get; set; }
        
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        // Add these properties
        [Required(ErrorMessage = "Semester is required")]
        [Range(1, 8, ErrorMessage = "Semester must be between 1 and 8")]
        public int Semester { get; set; }
        
        [Required(ErrorMessage = "Section is required")]
        [StringLength(1, ErrorMessage = "Section must be a single character")]
        [RegularExpression("[A-D]", ErrorMessage = "Section must be A, B, C, or D")]
        public string Section { get; set; }
        [Phone]
        public string? Phone { get; set; } = string.Empty;
        
        public string? RollNumber { get; set; }
        
        // New properties for authentication
        [NotMapped]
        public string? GeneratedStudentId { get; set; } // This will be the login ID
        
        [NotMapped]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        
        [NotMapped]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        // Navigation 
        public virtual ICollection<BookIssueViewModel> BookIssues { get; set; } = new List<BookIssueViewModel>();
        
    }
}