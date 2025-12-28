using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Practice_Project.Entities  // <-- Added: Wrap the class in this namespace
{
    public class Fine
    {
        [Key]
        public int Id { get; set; }  // <-- Changed: Renamed from FineId to Id (matches your controller/view)

        public int BookIssueId { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal Amount { get; set; }

        public DateTime GeneratedDate { get; set; } = DateTime.UtcNow;  // <-- Changed: Use UtcNow for consistency
        public DateTime? PaidDate { get; set; }
        public bool IsPaid { get; set; } = false;

        public virtual BookIssue BookIssue { get; set; } = null!;
    }
}