using System.ComponentModel.DataAnnotations;

namespace Practice_Project.Models;

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
    
    [Phone]
    public string? Phone { get; set; } = string.Empty;
    
    public string? RollNumber { get; set; }

    public bool IsActive { get; set; } = true;
    
    //Navigation 
    public virtual ICollection<BookIssue> BookIssues { get; set; } = new List<BookIssue>();
}