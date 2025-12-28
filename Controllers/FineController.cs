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
    public class FineController : Controller
    {
        private readonly LibraryDbContext _context;

        public FineController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: /Fine/ - List all fines
        public async Task<IActionResult> Index()
        {
            var fines = await _context.Fines
                .Include(f => f.BookIssue)
                    .ThenInclude(bi => bi.Book)
                .Include(f => f.BookIssue)
                    .ThenInclude(bi => bi.Student)
                .OrderByDescending(f => f.GeneratedDate)
                .ToListAsync();

            return View(fines);
        }

        // GET: /Fine/Create - Show form (optional manual fine)
        public IActionResult Create()
        {
            ViewBag.BookIssues = _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.Student)
                .Where(bi => bi.Status == "Returned" && bi.FineAmount > 0)
                .ToList();
                
            return View();
        }

        // POST: /Fine/Create - Manual fine creation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Fine fine)
        {
            if (ModelState.IsValid)
            {
                fine.GeneratedDate = DateTime.UtcNow;
                _context.Fines.Add(fine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.BookIssues = _context.BookIssues.ToList();
            return View(fine);
        }

        // GET: /Fine/Pay/5 - Mark fine as paid
        public async Task<IActionResult> Pay(int id)
        {
            var fine = await _context.Fines
                .Include(f => f.BookIssue)
                .FirstOrDefaultAsync(f => f.BookIssueId == id);

            if (fine == null || fine.IsPaid)
                return NotFound();

            return View(fine);
        }

        // POST: /Fine/Pay/5 - Confirm payment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(int id, Fine model)
        {
            var fine = await _context.Fines.FindAsync(id);
            if (fine == null)
                return NotFound();

            fine.IsPaid = true;
            fine.PaidDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Fine paid successfully!";
            return RedirectToAction(nameof(Index));
        }

        // Optional: Delete fine
        public async Task<IActionResult> Delete(int id)
        {
            var fine = await _context.Fines.FindAsync(id);
            if (fine == null)
                return NotFound();

            _context.Fines.Remove(fine);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}