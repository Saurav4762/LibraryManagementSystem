using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Practice_Project.Entities
{
    public class BookIssue
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Book is required")]
        [Display(Name = "Book")]
        public int BookId { get; set; }

        [Required(ErrorMessage = "Student is required")]
        [Display(Name = "Student")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Issue date is required")]
        [Display(Name = "Issue Date")]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Due date is required")]
        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(14);

        [Display(Name = "Return Date")]
        [DataType(DataType.Date)]
        public DateTime? ReturnDate { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 10, ErrorMessage = "Quantity must be between 1 and 10")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; } = 1;

        [Required(ErrorMessage = "Status is required")]
        [StringLength(50)]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Issued"; // Issued, Returned, Overdue, Lost

        [Display(Name = "Fine Amount")]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0, 1000, ErrorMessage = "Fine amount cannot exceed 1000")]
        public decimal FineAmount { get; set; } = 0;

        [Display(Name = "Remarks")]
        [StringLength(500)]
        public string Remarks { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "Updated At")]
        public DateTime? UpdatedAt { get; set; }

        // Foreign key relationships
        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; }

        // Calculated properties (not stored in database)
        [NotMapped]
        [Display(Name = "Days Overdue")]
        public int DaysOverdue
        {
            get
            {
                if (Status == "Issued" && DateTime.Now > DueDate)
                {
                    return (DateTime.Now - DueDate).Days;
                }
                else if (Status == "Returned" && ReturnDate.HasValue && ReturnDate > DueDate)
                {
                    return (ReturnDate.Value - DueDate).Days;
                }
                return 0;
            }
        }

        [NotMapped]
        [Display(Name = "Is Overdue")]
        public bool IsOverdue
        {
            get
            {
                return Status == "Issued" && DateTime.Now > DueDate;
            }
        }

        [NotMapped]
        [Display(Name = "Total Fine")]
        public decimal TotalFine
        {
            get
            {
                if (IsOverdue || (Status == "Returned" && DaysOverdue > 0))
                {
                    // Calculate fine: $1 per day overdue
                    return DaysOverdue * 1.00m;
                }
                return FineAmount;
            }
        }
    }
}