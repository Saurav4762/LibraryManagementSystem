/*using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practice_Project.Entities;
using Practice_Project.Models;
using System;
using System.Linq;
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
        public async Task<IActionResult> Index()
        {
            var issues = await _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.Student)
                .ToListAsync();

            return View(issues);
        }

        // GET: /BookIssue/Create - Show form
        public IActionResult Create()
        {
            ViewBag.Books = _context.Books?.ToList() ?? new List<Book>();
            ViewBag.Students = _context.Students?.ToList() ?? new List<Student>();

            return View(new BookIssueViewModel()); // Pass ViewModel
        }

        // POST: /BookIssue/Create - Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookIssueViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var issue = new BookIssue
                {
                    BookId = vm.BookId,
                    StudentId = vm.StudentId,
                    IssueDate = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(14),
                    Status = "Issued",
                    FineAmount = 0
                };

                _context.BookIssues.Add(issue);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Validation failed – reload dropdowns and return ViewModel
            ViewBag.Books = _context.Books?.ToList() ?? new List<Book>();
            ViewBag.Students = _context.Students?.ToList() ?? new List<Student>();

            return View(vm); // ← Must be vm (ViewModel), not entity
        }

        // GET: /BookIssue/Return/5
        public async Task<IActionResult> Return(int id)
        {
            var issue = await _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.Student)
                .FirstOrDefaultAsync(bi => bi.Id == id);

            if (issue == null)
                return NotFound();

            return View(issue);
        }

        // POST: /BookIssue/Return/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int id, BookIssue model)
        {
            var issue = await _context.BookIssues.FindAsync(id);
            if (issue == null)
                return NotFound();

            issue.ReturnDate = DateTime.UtcNow;
            issue.Status = "Returned";

            if (issue.ReturnDate > issue.DueDate)
            {
                int overdueDays = (issue.ReturnDate.Value - issue.DueDate).Days;
                issue.FineAmount = overdueDays * 1.00m;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}*/



using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practice_Project.Entities;
using Practice_Project.Models;
using System;
using System.Linq;
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

        // Index - unchanged
        public async Task<IActionResult> Index()
        {
            var issues = await _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.Student)
                .ToListAsync();

            return View(issues);
        }

        // GET Create - unchanged
        public IActionResult Create()
        {
            ViewBag.Books = _context.Books?.ToList() ?? new List<Book>();
            ViewBag.Students = _context.Students?.ToList() ?? new List<Student>();

            return View(new BookIssueViewModel());
        }

        // POST Create - Issue book + Decrease stock
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookIssueViewModel vm)
        {
            if (ModelState.IsValid)
            {
                // Check if book is available
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
                        DueDate = DateTime.UtcNow.AddDays(1),
                        Status = "Issued",
                        FineAmount = 0
                    };

                    // Decrease available quantity
                    book.QuantityAvailable -= 1;

                    _context.BookIssues.Add(issue);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
            }

            // If invalid or not available
            ViewBag.Books = _context.Books?.ToList() ?? new List<Book>();
            ViewBag.Students = _context.Students?.ToList() ?? new List<Student>();

            return View(vm);
        }

        // GET Return - unchanged
        public async Task<IActionResult> Return(int id)
        {
            var issue = await _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.Student)
                .FirstOrDefaultAsync(bi => bi.Id == id);

            if (issue == null)
                return NotFound();

            return View(issue);
        }

        /*// POST Return - Return book + Increase stock + Calculate fine
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int id, BookIssue model)
        {
            var issue = await _context.BookIssues
                .Include(bi => bi.Book)  // Important: load Book to update stock
                .FirstOrDefaultAsync(bi => bi.Id == id);

            if (issue == null)
                return NotFound();

            if (issue.Status == "Returned")
            {
                // Already returned – optional message
                TempData["Message"] = "This book is already returned.";
                return RedirectToAction(nameof(Index));
            }

            issue.ReturnDate = DateTime.UtcNow;
            issue.Status = "Returned";

            // Calculate fine if overdue
            if (issue.ReturnDate > issue.DueDate)
            {
                int overdueDays = (issue.ReturnDate.Value.Date - issue.DueDate.Date).Days;
                if (overdueDays > 0)
                {
                    issue.FineAmount = overdueDays * 5.00m;  // ₹5 per day (change as needed)
                }
            }

            // Increase book stock
            issue.Book.QuantityAvailable += 1;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Book returned successfully!";
            return RedirectToAction(nameof(Index));
        }*/
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int id, BookIssue model)
        {
            var issue = await _context.BookIssues
                .Include(bi => bi.Book)
                .FirstOrDefaultAsync(bi => bi.Id == id);

            if (issue == null) return NotFound();
            if (issue.Status == "Returned") { /* already returned */ }

            var returnDate = DateTime.UtcNow;
            decimal fine = 0m;
            int overdueDays = (returnDate.Date - issue.DueDate.Date).Days;
            if (overdueDays > 0)
                fine = overdueDays * 5.00m;

            // If there is a fine, show it and ask for confirmation/payment
            if (fine > 0)
            {
                ViewBag.CalculatedFine = fine;
                ViewBag.OverdueDays = overdueDays;
                // Return the same view with message: "Fine of ₹X must be collected"
                return View(issue);  // or a special "ConfirmFine" view
            }

            // Only if no fine → return immediately
            issue.ReturnDate = returnDate;
            issue.Status = "Returned";
            issue.FineAmount = fine;  // 0 in this case
            issue.Book.QuantityAvailable += 1;

            await _context.SaveChangesAsync();
            TempData["Success"] = "Book returned successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}