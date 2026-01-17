using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practice_Project.Data;
using Practice_Project.Entities;
using System.Threading.Tasks;

namespace Practice_Project.Controllers
{
public class FinePayController : Controller
{

        private readonly LibraryDbContext _context;

        public FinePayController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: /Fines - List all fines with student and book info
        public async Task<IActionResult> MainIndex()
        {
            var fines = await _context.Fines
                .Include(f => f.BookIssue)
                .ThenInclude(bi => bi.Book)
                .Include(f => f.BookIssue)
                .ThenInclude(bi => bi.Student)
                .Include(f => f.Student)
                .OrderByDescending(f => f.CalculatedOn)
                .ToListAsync();

            return View(fines);
        }
        // GET: /Fines/Details/5 - Full details of a fine (including return data)
        public async Task<IActionResult> ShowDetails(int? id)
        {
            if (id == null) return NotFound();

            var fine = await _context.Fines
                .Include(f => f.BookIssue)
                .ThenInclude(bi => bi.Book)
                .Include(f => f.BookIssue)
                .ThenInclude(bi => bi.Student)
                .Include(f => f.Student)
                .FirstOrDefaultAsync(f => f.Id == id);

            if (fine == null) return NotFound();

            return View(fine);
        }

        // Optional: Quick action to mark a fine as paid (if needed manually)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsPaid(int id)
        {
            var fine = await _context.Fines.FindAsync(id);
            if (fine == null) return NotFound();

            if (!fine.IsPaid)
            {
                fine.IsPaid = true;
                fine.PaidOn = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }

            TempData["Success"] = $"Fine marked as paid on {fine.PaidOn:dd MMM yyyy}.";
            return RedirectToAction(nameof(MainIndex));
        }

        // Helper
        private bool FineExists(int id) => _context.Fines.Any(e => e.Id == id);
    }
}