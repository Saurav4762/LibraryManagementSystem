using System.ComponentModel.DataAnnotations;

namespace Practice_Project.Models;

public class Category
{
    [Key]
    public int CategoryId { get; set; }

    [Required] [StringLength(50)] 
    public string Name { get; set; } = string.Empty;
    
    public string? Description { get; set; }
    
    // Navigation
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    

}