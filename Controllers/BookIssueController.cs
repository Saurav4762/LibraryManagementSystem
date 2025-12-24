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
                    IssueDate = DateTime.Now,
                    DueDate = DateTime.Now.AddDays(14),
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

            issue.ReturnDate = DateTime.Now;
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
}