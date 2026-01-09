using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practice_Project.Entities;
using Practice_Project.Models;
using System;
using System.Threading.Tasks;
using Practice_Project.Data;

namespace Practice_Project.Controllers
{
    public class BookIssueController : Controller
    {
        private readonly LibraryDbContext _context;

        public BookIssueController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: /BookIssue/
        public async Task<IActionResult> Index(string searchString)
        {
            var issues = _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.Fines)
                .Include(bi => bi.Student)
                .AsQueryable();

            // 🔍 Search by Student Name OR Book Title
            if (!string.IsNullOrEmpty(searchString))
            {
                issues = issues.Where(bi =>
                    bi.Student.Name.Contains(searchString) ||
                    bi.Book.Title.Contains(searchString));
            }

            return View(await issues.ToListAsync());
        }


        // GET: /BookIssue/Create
        public IActionResult Create()
        {
            ViewBag.Books = _context.Books?.ToList() ?? new List<Book>();
            ViewBag.Students = _context.Students?.ToList() ?? new List<Student>();

            return View(new BookIssueViewModel());
        }

        // POST: /BookIssue/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookIssueViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var book = await _context.Books.FindAsync(vm.BookId);
                if (book == null || book.QuantityAvailable <= 0)
                {
                    ModelState.AddModelError("", "This book is not available for issue.");
                }
                else
                {
                    var issue = new BookIssue
                    {
                        BookId = vm.BookId,
                        StudentId = vm.StudentId,
                        IssueDate = DateTime.UtcNow,
                        DueDate = DateTime.UtcNow.AddDays(14), // or keep 1 day for testing
                        Status = "Issued",
                        FineAmount = 0
                    };

                    book.QuantityAvailable -= 1;
                    _context.BookIssues.Add(issue);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }

            ViewBag.Books = _context.Books?.ToList() ?? new List<Book>();
            ViewBag.Students = _context.Students?.ToList() ?? new List<Student>();
            return View(vm);
        }

        // ==================== RETURN ACTIONS ====================

        // GET: /BookIssue/Return/5 → Show details before returning
        public async Task<IActionResult> Return(int id)
        {
            var issue = await _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.Student)
                .FirstOrDefaultAsync(bi => bi.Id == id && bi.Status == "Issued");

            if (issue == null)
            {
                TempData["Error"] = "Book issue not found or already returned.";
                return RedirectToAction(nameof(Index));
            }

            return View(issue);
        }

        // POST: /BookIssue/Return/5 → Handle the actual return logic
      // POST: /BookIssue/Return/5 → Handle the actual return logic
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Return(int id, string action = null)
{
    var issue = await _context.BookIssues
        .Include(bi => bi.Book)
        .Include(bi => bi.Student)
        .FirstOrDefaultAsync(bi => bi.Id == id);

    if (issue == null)
        return NotFound();

    if (issue.Status == "Returned")
    {
        TempData["Info"] = "This book has already been returned.";
        return RedirectToAction(nameof(Index));
    }

    var returnDate = DateTime.UtcNow;
    int overdueDays = Math.Max(0, (returnDate.Date - issue.DueDate.Date).Days);
    decimal fineAmount = overdueDays * 5.00m; // ₹5 per day

    // Case 1: Confirmation received (admin clicked "Collect Fine & Return")
    if (action == "confirm")
    {
        issue.ReturnDate = returnDate;
        issue.Status = "Returned";
        issue.FineAmount = fineAmount;

        // Return book to stock
        issue.Book.QuantityAvailable += 1;

        // Record fine in Fines table if overdue
        if (fineAmount > 0)
        {
            var fine = new Fine
            {
                BookIssueId = issue.Id,
                StudentId = issue.StudentId,
                Amount = fineAmount,
                CalculatedOn = returnDate,
                IsPaid = true,
                PaidOn = returnDate  // ← Now correctly records when fine was paid
            };
            _context.Fines.Add(fine);
        }

        await _context.SaveChangesAsync();

        TempData["Success"] = fineAmount > 0
            ? $"Book returned successfully. Fine of ₹{fineAmount} collected ({overdueDays} days overdue)."
            : "Book returned successfully with no fine.";

        return RedirectToAction(nameof(Index));
    }

    // Case 2: Overdue → Show confirmation page
    if (fineAmount > 0)
    {
        ViewBag.CalculatedFine = fineAmount;
        ViewBag.OverdueDays = overdueDays;
        ViewBag.ReturnDate = returnDate.ToString("dd MMM yyyy");

        return View("ReturnConfirm", issue);
    }

    // Case 3: No fine → Return immediately
    issue.ReturnDate = returnDate;
    issue.Status = "Returned";
    issue.FineAmount = 0;
    issue.Book.QuantityAvailable += 1;

    await _context.SaveChangesAsync();

    TempData["Success"] = "Book returned successfully!";
    return RedirectToAction(nameof(Index));
}
    }
}