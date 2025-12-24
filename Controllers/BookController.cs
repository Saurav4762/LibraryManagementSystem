// Controllers/BooksController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Practice_Project.Data;
using Practice_Project.Entities;
using Practice_Project.Models;

namespace Practice_Project.Controllers
{
    public class BooksController : Controller
    {
        private readonly LibraryDbContext _context;

        public BooksController(LibraryDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var books = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .OrderBy(b => b.Title)
                .ToListAsync();

            
            
            return View(books);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View(new BookViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookViewModel model)
        {
            // Extra safety: prevent Available > Total
            if (model.QuantityAvailable > model.TotalQuantity)
                ModelState.AddModelError("QuantityAvailable", "Available copies cannot exceed total copies.");

            if (ModelState.IsValid)
            {
                var book = new Book
                {
                    Title = model.Title,
                    ISBN = model.ISBN,
                    PublicationYear = model.PublicationYear,
                    TotalQuantity = model.TotalQuantity,
                    QuantityAvailable = model.QuantityAvailable,
                    PublicationDate = model.PublicationDate.HasValue
                        ? DateTime.SpecifyKind(model.PublicationDate.Value, DateTimeKind.Utc)
                        : null,
                    IsActive = model.IsActive,
                    AuthorId = model.AuthorId,
                    CategoryId = model.CategoryId
                };

                _context.Books.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            await LoadDropdowns(model.AuthorId, model.CategoryId);
            return View(model);
        }


public async Task<IActionResult> Edit(int? id)
{
    if (id == null) return NotFound();

    var book = await _context.Books.FindAsync(id);
    if (book == null) return NotFound();

    var model = new BookViewModel
    {
        BookId = book.Id,
        Title = book.Title,
        ISBN = book.ISBN,
        PublicationYear = book.PublicationYear,
        TotalQuantity = book.TotalQuantity,
        QuantityAvailable = book.QuantityAvailable,
        PublicationDate = book.PublicationDate,
        IsActive = book.IsActive,
        AuthorId = book.AuthorId,
        CategoryId = book.CategoryId
    };

    await LoadDropdownsAsync(model);
    return View(model);
}

[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, BookViewModel model)
{
    if (id != model.BookId) return NotFound();

    // Custom business rule
    if (model.QuantityAvailable > model.TotalQuantity)
        ModelState.AddModelError("QuantityAvailable", "Available copies cannot exceed total copies.");

    if (ModelState.IsValid)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null) return NotFound();

        // Update all fields
        book.Title = model.Title;
        book.ISBN = model.ISBN;
        book.PublicationYear = model.PublicationYear;
        book.TotalQuantity = model.TotalQuantity;
        book.QuantityAvailable = model.QuantityAvailable;
        book.PublicationDate = model.PublicationDate.HasValue
            ? DateTime.SpecifyKind(model.PublicationDate.Value, DateTimeKind.Utc)
            : null;
        book.IsActive = model.IsActive;
        book.AuthorId = model.AuthorId;
        book.CategoryId = model.CategoryId;

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // IMPORTANT: Reload dropdowns when validation fails
    await LoadDropdownsAsync(model);
    return View(model);
}

// NEW: Clean helper that populates the ViewModel directly
private async Task LoadDropdownsAsync(BookViewModel model)
{
    model.Authors = await _context.Authors
        .OrderBy(a => a.Name)
        .Select(a => new SelectListItem
        {
            Value = a.AuthorId.ToString(),
            Text = a.Name,
            Selected = a.AuthorId == model.AuthorId
        })
        .ToListAsync();

    model.Categories = await _context.Categories
        .OrderBy(c => c.Name)
        .Select(c => new SelectListItem
        {
            Value = c.CategoryId.ToString(),
            Text = c.Name,
            Selected = c.CategoryId == model.CategoryId
        })
        .ToListAsync();
}
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Category)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null) return NotFound();
            return View(book);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdowns(int selectedAuthorId = 0, int selectedCategoryId = 0)
        {
            ViewBag.AuthorId = new SelectList(
                await _context.Authors.OrderBy(a => a.Name).ToListAsync(),
                "AuthorId", "Name", selectedAuthorId);

            ViewBag.CategoryId = new SelectList(
                await _context.Categories.OrderBy(c => c.Name).ToListAsync(),
                "CategoryId", "Name", selectedCategoryId);
        }

        private bool BookExits(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}