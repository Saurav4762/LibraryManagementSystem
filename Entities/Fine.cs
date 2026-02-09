/*using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Practice_Project.Entities
{
    [Table("Fines")]
    public class Fine
    {
        [Key]
        public int Id { get; set; }

        public int BookIssueId { get; set; }

        [ForeignKey("BookIssueId")]
        public BookIssue BookIssue { get; set; }

        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public Student Student { get; set; }

        public decimal Amount { get; set; }

        public DateTime CalculatedOn { get; set; } = DateTime.UtcNow;

        public bool IsPaid { get; set; } = false;

        public DateTime? PaidOn { get; set; }

        public string? PaidBy { get; set; } // e.g., Librarian name or ID
    }
}*/

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Practice_Project.Entities
{
    [Table("Fines")]
    public class Fine
    {
        [Key]
        public int Id { get; set; }

        public int BookIssueId { get; set; }

        [ForeignKey(nameof(BookIssueId))]
        public BookIssue BookIssue { get; set; } = null!;

        public int StudentId { get; set; }

        [ForeignKey(nameof(StudentId))]
        public Student Student { get; set; } = null!;

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        public DateTime CalculatedOn { get; set; } = DateTime.UtcNow;

        public bool IsPaid { get; set; } = false;

        public DateTime? PaidOn { get; set; }

        public string? PaidBy { get; set; }
    }
}