
    
// Controllers/AuthorsController.cs

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practice_Project.Data;
using Practice_Project.Entities;
using Practice_Project.Filters;
using Practice_Project.Models;

namespace Practice_Project.Controllers
{
    [StaffOnly]
    public class AuthorsController : BaseController
    {
        private readonly LibraryDbContext _context;

        public AuthorsController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Authors → List all authors (with book count)
        public async Task<IActionResult> Index()
        {
            var authors = await _context.Authors
                .Include(a => a.Books)           // Needed for @author.Books.Count in Index view
                .OrderBy(a => a.Name)
                .ToListAsync();

            return View(authors); // Returns List<Author> → matches your Index.cshtml
        }

        // GET: Authors/Create
        public IActionResult Create()
        {
            return View(new AuthorViewModel());
        }

        // POST: Authors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AuthorViewModel model)
        {
            if (ModelState.IsValid)
            {
                var author = new Author
                {
                    Name = model.Name,
                    Biography = model.Biography
                };

                _context.Authors.Add(author);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var author = await _context.Authors.FindAsync(id);
            if (author == null) return NotFound();

            var model = new AuthorViewModel
            {
                AuthorId = author.AuthorId,
                Name = author.Name,
                Biography = author.Biography
            };

            return View(model);
        }

        // POST: Authors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AuthorViewModel model)
        {
            if (id != model.AuthorId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var author = await _context.Authors.FindAsync(id);
                    if (author == null) return NotFound();

                    author.Name = model.Name;
                    author.Biography = model.Biography;

                    _context.Update(author);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(id)) return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Authors/Delete/5
        // GET: Authors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var author = await _context.Authors
                .Include(a => a.Books)                 // ← THIS LINE IS MISSING!
                .FirstOrDefaultAsync(m => m.AuthorId == id);

            if (author == null) return NotFound();

            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author != null)
            {
                _context.Authors.Remove(author);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.AuthorId == id);
        }
    }
}