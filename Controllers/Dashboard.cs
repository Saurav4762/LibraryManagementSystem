/*
namespace Practice_Project.Controllers;

public class Dashboard
{

}
*/


using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // If using DbContext for stats
using  Practice_Project.Data; // Replace with your namespace (e.g., Practice_Project.Data)

namespace Practice_Project.Controllers // Adjust namespace if different
{
    public class DashboardController : Controller
    {
        private readonly LibraryDbContext _context; // Assume your DbContext is named this

        public DashboardController(LibraryDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Fetch counts for dashboard widgets
            ViewBag.TotalBooks = await _context.Books.CountAsync();
            ViewBag.TotalStudents = await _context.Students.CountAsync(); // Or Members/Users if different
            ViewBag.TotalCategories = await _context.Categories.CountAsync();
            ViewBag.TotalAuthors = await _context.Authors.CountAsync();
            ViewBag.TotalIssuedBooks = await _context.BookIssues.CountAsync(); // All issues (borrowed books)
            ViewBag.CurrentlyBorrowed = await _context.BookIssues
                .Where(i => i.ReturnDate == null) // Assuming null means not returned yet
                .CountAsync();
            return View();
        }
    }
}