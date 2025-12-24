using System.ComponentModel.DataAnnotations;

namespace Practice_Project.Entities
{
    public class BookIssue
    {
        [Key]
        public int Id { get; set; }

        // ADD [Required] HERE
        [Required(ErrorMessage = "Please select a book")]
        public int BookId { get; set; }

        [Required(ErrorMessage = "Please select a student")]
        public int StudentId { get; set; }

        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }

        public string Status { get; set; } = "Issued";
        public decimal FineAmount { get; set; } = 0;

        public virtual Book Book { get; set; } = null!;
        public virtual Student Student { get; set; } = null!;
    }
}