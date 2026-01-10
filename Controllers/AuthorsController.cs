using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practice_Project.Data;
using Practice_Project.Entities;
using Practice_Project.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Practice_Project.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly LibraryDbContext _context;

        public AuthorsController(LibraryDbContext context)
        {
            _context = context;
        }

        // GET: Authors → List all authors (with book count)
        public async Task<IActionResult> Index(string searchString)
        {
            // Base SQL query for authors
            string sql = "SELECT * FROM \"Authors\"";

            // Add WHERE clause if searchString is provided
            if (!string.IsNullOrEmpty(searchString))
            {
                // Use parameterized query to prevent SQL injection
                sql += " WHERE \"Name\" ILIKE {0}"; // ILIKE for case-insensitive search in PostgreSQL
                searchString = $"%{searchString}%";
            }

            // Always order by Name
            sql += " ORDER BY \"Name\"";

            // Execute query
            var authors = string.IsNullOrEmpty(searchString)
                ? await _context.Authors.FromSqlRaw(sql).ToListAsync()
                : await _context.Authors.FromSqlRaw(sql, searchString).ToListAsync();

            // Load books for each author to get book count
            foreach (var author in authors)
            {
                author.Books = await _context.Books
                    .FromSqlRaw("SELECT * FROM \"Books\" WHERE \"AuthorId\" = {0}", author.AuthorId)
                    .ToListAsync();
            }

            return View(authors);
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
                // Manual INSERT SQL
                await _context.Database.ExecuteSqlRawAsync(
                    "INSERT INTO \"Authors\" (\"Name\", \"Biography\") VALUES ({0}, {1})",
                    model.Name, model.Biography
                );

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            // Manual SQL to get author by ID
            var author = await _context.Authors
                .FromSqlRaw("SELECT * FROM \"Authors\" WHERE \"AuthorId\" = {0}", id)
                .FirstOrDefaultAsync();

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
                // Manual UPDATE SQL
                await _context.Database.ExecuteSqlRawAsync(
                    "UPDATE \"Authors\" SET \"Name\" = {0}, \"Biography\" = {1} WHERE \"AuthorId\" = {2}",
                    model.Name, model.Biography, model.AuthorId
                );

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Authors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            // Load author manually
            var author = await _context.Authors
                .FromSqlRaw("SELECT * FROM \"Authors\" WHERE \"AuthorId\" = {0}", id)
                .FirstOrDefaultAsync();

            if (author == null) return NotFound();

            // Load books for display (optional)
            author.Books = await _context.Books
                .FromSqlRaw("SELECT * FROM \"Books\" WHERE \"AuthorId\" = {0}", author.AuthorId)
                .ToListAsync();

            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Manual DELETE SQL
            await _context.Database.ExecuteSqlRawAsync(
                "DELETE FROM \"Authors\" WHERE \"AuthorId\" = {0}", id
            );

            return RedirectToAction(nameof(Index));
        }

        // Optional: Check existence using manual SQL
        private async Task<bool> AuthorExists(int id)
        {
            var exists = await _context.Authors
                .FromSqlRaw("SELECT * FROM \"Authors\" WHERE \"AuthorId\" = {0}", id)
                .AnyAsync();

            return exists;
        }
    }
}
