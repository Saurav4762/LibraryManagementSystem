using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practice_Project.Data; // Replace with your actual DbContext namespace
using Practice_Project.Entities;

namespace Practice_Project.Controllers
{
    public class FinesController : Controller
    {
        private readonly LibraryDbContext _context; // Change to your actual DbContext name

        public FinesController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Fines - List all fines with related data
        public async Task<IActionResult> Index()
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

        // GET: Fines/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var fine = await _context.Fines
                .Include(f => f.BookIssue).ThenInclude(bi => bi.Book)
                .Include(f => f.BookIssue).ThenInclude(bi => bi.Student)
                .Include(f => f.Student)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (fine == null) return NotFound();

            return View(fine);
        }

        // GET: Fines/Create
        public IActionResult Create()
        {
            ViewBag.BookIssues = _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.Student)
                .Where(bi => bi.ReturnDate == null || bi.ReturnDate > bi.DueDate) // Only overdue or issued
                .Select(bi => new
                {
                    bi.Id,
                    DisplayText = $"Book: {bi.Book.Title} - Student: {bi.Student.Name} (Due: {bi.DueDate:yyyy-MM-dd})"
                })
                .ToList();

            ViewBag.Students = _context.Students.ToList();
            return View();
        }

        // POST: Fines/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Fine fine)
        {
            if (ModelState.IsValid)
            {
                fine.CalculatedOn = DateTime.UtcNow;
                _context.Add(fine);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Repopulate dropdowns if validation fails
            ViewBag.BookIssues = _context.BookIssues.Include(bi => bi.Book).Include(bi => bi.Student).ToList();
            ViewBag.Students = _context.Students.ToList();
            return View(fine);
        }

        // GET: Fines/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var fine = await _context.Fines.FindAsync(id);
            if (fine == null) return NotFound();

            ViewBag.BookIssues = _context.BookIssues.Include(bi => bi.Book).Include(bi => bi.Student).ToList();
            ViewBag.Students = _context.Students.ToList();
            return View(fine);
        }

        // POST: Fines/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Fine fine)
        {
            if (id != fine.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (fine.IsPaid && fine.PaidOn == null)
                        fine.PaidOn = DateTime.UtcNow;

                    _context.Update(fine);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FineExists(fine.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.BookIssues = _context.BookIssues.Include(bi => bi.Book).Include(bi => bi.Student).ToList();
            ViewBag.Students = _context.Students.ToList();
            return View(fine);
        }

        // GET: Fines/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var fine = await _context.Fines
                .Include(f => f.BookIssue).ThenInclude(bi => bi.Book)
                .Include(f => f.BookIssue).ThenInclude(bi => bi.Student)
                .Include(f => f.Student)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (fine == null) return NotFound();

            return View(fine);
        }

        // POST: Fines/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fine = await _context.Fines.FindAsync(id);
            if (fine != null)
            {
                _context.Fines.Remove(fine);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Action to mark fine as paid (bonus shortcut)
        public async Task<IActionResult> MarkAsPaid(int id, string paidBy = "Admin")
        {
            var fine = await _context.Fines.FindAsync(id);
            if (fine == null) return NotFound();

            fine.IsPaid = true;
            fine.PaidOn = DateTime.UtcNow;
            fine.PaidBy = paidBy;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FineExists(int id)
        {
            return _context.Fines.Any(e => e.Id == id);
        }
    }
}