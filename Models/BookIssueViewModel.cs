using System.ComponentModel.DataAnnotations;

namespace Practice_Project.Models;

public class BookIssueViewModel
{
    [Key]
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Please select a book")]
    public int BookId { get; set; }
    [Required(ErrorMessage = "Please select a book")]
    public int StudentId { get; set; }
    
    public DateTime IssueDate { get; set; } = DateTime.Now;
    
    public DateTime DueDate {get; set;}
    
    public DateTime? ReturnDate {get; set;}

    public string Status { get; set; } = "Issued";

    public decimal FineAmount { get; set; } = 0;
    
    //Navigation 
    public virtual BookViewModel Bookvm { get; set; } = null!;
    public virtual StudentVm StudentVm { get; set; } = null!;
}