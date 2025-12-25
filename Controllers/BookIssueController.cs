using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practice_Project.Entities;
using Practice_Project.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Practice_Project.Data;

namespace Practice_Project.Controllers
{
    [Authorize]
    public class BookIssueController : BaseController
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
                .OrderByDescending(bi => bi.IssueDate)
                .ToListAsync();

            return View(issues);
        }

        // GET: /BookIssue/Create - Show form
       public async Task<IActionResult> Create()
       {
           
               try
               {
                   // Get books with availability check
                   var availableBooks = await _context.Books
                       .Where(b => b.IsActive && b.QuantityAvailable > 0)
                       .Select(b => new
                       {
                           b.Id,
                           b.Title,
                           b.QuantityAvailable
                       })
                       .ToListAsync();

                   // Get active students
                   var activeStudents = await _context.Students
                       .Where(s => s.IsActive)
                       .Select(s => new
                       {
                           s.StudentId,
                           s.Name,
                           s.RollNumber
                       })
                       .ToListAsync();

                   // Create SelectListItems - CRITICAL: Never pass null to ViewBag
                   var bookItems = availableBooks.Select(b => new SelectListItem
                   {
                       Value = b.Id.ToString(),
                       Text = $"{b.Title} (Available: {b.QuantityAvailable})"
                   }).ToList();

                   var studentItems = activeStudents.Select(s => new SelectListItem
                   {
                       Value = s.StudentId.ToString(),
                       Text = $"{s.Name} - {s.RollNumber}"
                   }).ToList();

                   // ALWAYS assign, even if empty
                   ViewBag.Books = bookItems.Any()
                       ? new SelectList(bookItems, "Value", "Text")
                       : new SelectList(new List<SelectListItem>());

                   ViewBag.Students = studentItems.Any()
                       ? new SelectList(studentItems, "Value", "Text")
                       : new SelectList(new List<SelectListItem>());

                   // Create ViewModel
                   var viewModel = new BookIssueViewModel
                   {
                       IssueDate = DateTime.Today,
                       DueDate = DateTime.Today.AddDays(14),
                       Status = "Issued",
                       Quantity = 1
                   };

                   // Add warnings if no data
                   if (!bookItems.Any())
                   {
                       TempData["BookWarning"] = "No books available. Please add books first.";
                   }

                   if (!studentItems.Any())
                   {
                       TempData["StudentWarning"] = "No students available. Please register students first.";
                   }

                   return View(viewModel);
               }
               catch (Exception ex)
               {
                   TempData["ErrorMessage"] = $"Error loading form: {ex.Message}";
                   return RedirectToAction(nameof(Index));
           }
       }

        // POST: /BookIssue/Create - Save
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookIssueViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // Repopulate dropdowns on validation error
                await PopulateDropdowns();
                return View(vm);
            }

            try
            {
                // Check if book exists and is available
                var book = await _context.Books.FindAsync(vm.BookId);
                if (book == null)
                {
                    ModelState.AddModelError("BookId", "Selected book not found.");
                    await PopulateDropdowns();
                    return View(vm);
                }

                if (!book.IsActive)
                {
                    ModelState.AddModelError("BookId", "Selected book is not active.");
                    await PopulateDropdowns();
                    return View(vm);
                }

                if (book.QuantityAvailable < vm.Quantity)
                {
                    ModelState.AddModelError("Quantity", $"Only {book.QuantityAvailable} copies available.");
                    await PopulateDropdowns();
                    return View(vm);
                }

                // Check if student exists and is active
                var student = await _context.Students.FindAsync(vm.StudentId);
                if (student == null)
                {
                    ModelState.AddModelError("StudentId", "Selected student not found.");
                    await PopulateDropdowns();
                    return View(vm);
                }

                if (!student.IsActive)
                {
                    ModelState.AddModelError("StudentId", "Selected student is not active.");
                    await PopulateDropdowns();
                    return View(vm);
                }

                var issue = new BookIssue
                {
                    BookId = vm.BookId,
                    StudentId = vm.StudentId,
                    IssueDate = vm.IssueDate,
                    DueDate = vm.DueDate,
                    ReturnDate = vm.ReturnDate,
                    Quantity = vm.Quantity,
                    Status = vm.Status,
                    Remarks = vm.Remarks,
                    FineAmount = 0,
                    CreatedAt = DateTime.UtcNow
                };

                // Update book available quantity
                book.QuantityAvailable -= vm.Quantity;
                _context.Books.Update(book);

                _context.BookIssues.Add(issue);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Book issued successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error issuing book: {ex.Message}");
                await PopulateDropdowns();
                return View(vm);
            }
        }

        // Helper method to populate dropdowns
        private async Task PopulateDropdowns()
        {
            var availableBooks = await _context.Books
                .Where(b => b.IsActive && b.QuantityAvailable > 0)
                .OrderBy(b => b.Title)
                .ToListAsync();

            var activeStudents = await _context.Students
                .Where(s => s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync();

            ViewBag.Books = availableBooks.Any() 
                ? new SelectList(availableBooks, "Id", "Title") 
                : new SelectList(new List<Book>());

            ViewBag.Students = activeStudents.Any()
                ? new SelectList(activeStudents, "Id", "Name")
                : new SelectList(new List<Student>());
        }

        // GET: /BookIssue/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var issue = await _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.Student)
                .FirstOrDefaultAsync(bi => bi.Id == id);

            if (issue == null)
                return NotFound();

            return View(issue);
        }

        // GET: /BookIssue/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var issue = await _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.Student)
                .FirstOrDefaultAsync(bi => bi.Id == id);

            if (issue == null)
                return NotFound();

            var viewModel = new BookIssueViewModel
            {
                Id = issue.Id,
                BookId = issue.BookId,
                StudentId = issue.StudentId,
                IssueDate = issue.IssueDate,
                DueDate = issue.DueDate,
                ReturnDate = issue.ReturnDate,
                Quantity = issue.Quantity,
                Status = issue.Status,
                Remarks = issue.Remarks
            };

            await PopulateDropdowns();
            return View(viewModel);
        }

        // POST: /BookIssue/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, BookIssueViewModel vm)
        {
            if (id != vm.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                await PopulateDropdowns();
                return View(vm);
            }

            try
            {
                var existingIssue = await _context.BookIssues.FindAsync(id);
                if (existingIssue == null)
                    return NotFound();

                // Update book quantity if changed
                if (existingIssue.BookId != vm.BookId || existingIssue.Quantity != vm.Quantity)
                {
                    // Return old book quantity
                    var oldBook = await _context.Books.FindAsync(existingIssue.BookId);
                    if (oldBook != null)
                    {
                        oldBook.QuantityAvailable += existingIssue.Quantity;
                        _context.Books.Update(oldBook);
                    }

                    // Deduct new book quantity
                    var newBook = await _context.Books.FindAsync(vm.BookId);
                    if (newBook != null)
                    {
                        if (newBook.QuantityAvailable < vm.Quantity)
                        {
                            ModelState.AddModelError("Quantity", $"Only {newBook.QuantityAvailable} copies available.");
                            await PopulateDropdowns();
                            return View(vm);
                        }
                        newBook.QuantityAvailable -= vm.Quantity;
                        _context.Books.Update(newBook);
                    }
                }

                // Update issue
                existingIssue.BookId = vm.BookId;
                existingIssue.StudentId = vm.StudentId;
                existingIssue.IssueDate = vm.IssueDate;
                existingIssue.DueDate = vm.DueDate;
                existingIssue.ReturnDate = vm.ReturnDate;
                existingIssue.Quantity = vm.Quantity;
                existingIssue.Status = vm.Status;
                existingIssue.Remarks = vm.Remarks;
                existingIssue.UpdatedAt = DateTime.UtcNow;

                _context.BookIssues.Update(existingIssue);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Book issue updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating book issue: {ex.Message}");
                await PopulateDropdowns();
                return View(vm);
            }
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

            // Auto-calculate fine if overdue
            if (issue.Status == "Issued" && DateTime.UtcNow > issue.DueDate)
            {
                int overdueDays = (DateTime.UtcNow - issue.DueDate).Days;
                issue.FineAmount = overdueDays * 1.00m;
            }

            return View(issue);
        }

        // POST: /BookIssue/Return/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Return(int id, BookIssue model)
        {
            var issue = await _context.BookIssues
                .Include(bi => bi.Book)
                .FirstOrDefaultAsync(bi => bi.Id == id);

            if (issue == null)
                return NotFound();

            try
            {
                // Return book quantity
                var book = issue.Book;
                if (book != null)
                {
                    book.QuantityAvailable += issue.Quantity;
                    _context.Books.Update(book);
                }

                // Update issue
                issue.ReturnDate = model.ReturnDate ?? DateTime.UtcNow;
                issue.Status = "Returned";
                issue.Remarks = model.Remarks;
                issue.FineAmount = model.FineAmount;
                issue.UpdatedAt = DateTime.UtcNow;

                // Calculate fine if overdue
                if (issue.ReturnDate > issue.DueDate)
                {
                    int overdueDays = (issue.ReturnDate.Value - issue.DueDate).Days;
                    issue.FineAmount = overdueDays * 1.00m;
                }

                _context.BookIssues.Update(issue);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Book returned successfully! Fine: ${issue.FineAmount:F2}";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error returning book: {ex.Message}";
                return View(issue);
            }
        }

        // GET: /BookIssue/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var issue = await _context.BookIssues
                .Include(bi => bi.Book)
                .Include(bi => bi.Student)
                .FirstOrDefaultAsync(bi => bi.Id == id);

            if (issue == null)
                return NotFound();

            return View(issue);
        }

        // POST: /BookIssue/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var issue = await _context.BookIssues
                .Include(bi => bi.Book)
                .FirstOrDefaultAsync(bi => bi.Id == id);

            if (issue == null)
                return NotFound();

            try
            {
                // Return book quantity if not returned
                if (issue.Status != "Returned" && issue.Book != null)
                {
                    issue.Book.QuantityAvailable += issue.Quantity;
                    _context.Books.Update(issue.Book);
                }

                _context.BookIssues.Remove(issue);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Book issue deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting book issue: {ex.Message}";
                return View("Delete", issue);
            }
        }
    }
}