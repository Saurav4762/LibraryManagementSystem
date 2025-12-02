using System.ComponentModel.DataAnnotations;

namespace Practice_Project.Models;

public class Fine
{
    [Key]
    public int Id { get; set; }

    public int BookIssueId { get; set; }

    public decimal Amount { get; set; }

    public DateTime GeneratedDate { get; set; } = DateTime.Now;

    public DateTime? PaidDate { get; set; }

    public bool IsPaid { get; set; } = false;

    // Navigation
    public virtual BookIssue BookIssue { get; set; } = null!;
}