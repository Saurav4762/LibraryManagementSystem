using System.ComponentModel.DataAnnotations;

namespace Practice_Project.Models;

public class BookIssue
{
    [Key]
    public int Id { get; set; }
    
    public int BookId { get; set; }
    
    public int StudentId { get; set; }
    
    public DateTime IssueDate { get; set; } = DateTime.Now;
    
    public DateTime DueDate {get; set;}
    
    public DateTime? ReturnDate {get; set;}

    public string Status { get; set; } = "Issued";

    public decimal FineAmount { get; set; } = 0;
    
    //Navigation 
    public virtual Book Book { get; set; } = null!;
    public virtual Student Student { get; set; } = null!;
}