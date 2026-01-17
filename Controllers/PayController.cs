using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practice_Project.Data;
using Practice_Project.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace Practice_Project.Controllers
{
    public class PayController : Controller
    {
        private readonly LibraryDbContext _context;

        public PayController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: /Pay?searchString=StudentName
        public async Task<IActionResult> Index(string searchString)
        {
            // Base query: only paid fines
            var finesQuery = _context.Fines
                .Include(f => f.Student)
                .Include(f => f.BookIssue)
                .ThenInclude(bi => bi.Book)
                .Where(f => f.IsPaid == true)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchString))
            {
                // Convert both sides to lowercase for case-insensitive search
                var searchLower = searchString.ToLower();

                finesQuery = finesQuery.Where(f =>
                        f.Student.Name.ToLower().Contains(searchLower) ||   // Student name flexible
                        f.BookIssue.Book.Title.ToLower().Contains(searchLower) // Optional: search by book too
                );
            }

            var fines = await finesQuery
                .OrderByDescending(f => f.BookIssue.IssueDate)
                .ToListAsync();

            ViewData["CurrentFilter"] = searchString;

            return View(fines);
        }

        /*public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var fine = await _context.Fines
                .Include(f => f.Student)
                .Include(f => f.BookIssue)
                .ThenInclude(bi => bi.Book)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fine == null)
                return NotFound();

            return View(fine);
        }*/
    }
}