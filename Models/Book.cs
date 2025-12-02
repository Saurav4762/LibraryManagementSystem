using System.ComponentModel.DataAnnotations;

namespace Practice_Project.Models;

public class Book
{
    [Key]
    public int BookId { get; set; }

    [Required] [StringLength(50)] public string Title { get; set; } = string.Empty;
    
    [Required]
    [StringLength(13)]
    public string ISBN { get; set; } = string.Empty;
    
    public int PublicationYear { get; set; }
    
    public int TotalQuantity { get; set; }
    public int QuantityAvailable { get; set; }
    public DateTime PublicationDate { get; set; } = DateTime.Now;
    
    //Foreign Keys
    public int AuthorId { get; set; }
    public int CategoryId { get; set; }
    
    //Navigation Properties
    public virtual Author Author { get; set; } = null!;
    public virtual Category Category { get; set; } = null!;
    public virtual ICollection<BookIssue> BookIssues { get; set; } = new List<BookIssue>();

}