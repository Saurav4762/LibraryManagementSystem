using System.ComponentModel.DataAnnotations;

namespace Practice_Project.Models;

public class Author
{
    [Key]
    public int AuthorId { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;
    
    public string? Biography { get; set; }
    
    //Navigation
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
    
}