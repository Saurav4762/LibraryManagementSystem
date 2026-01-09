using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Practice_Project.Data;
using Practice_Project.Entities;
using Practice_Project.Models;

namespace Practice_Project.Controllers;

public class StudentController : Controller
{
    private readonly LibraryDbContext _context;

    public StudentController(LibraryDbContext context) => _context = context;

    public async Task<IActionResult> Index(string searchString)
    {
        var students = _context.Students.AsQueryable();

        // Search by Name
        if (!string.IsNullOrEmpty(searchString))
        {
            students = students
                .Where(s => s.Name.Contains(searchString));
        }

        students = students.OrderBy(s => s.Name);

        return View(await students.ToListAsync());
    }

  

    public async Task<IActionResult> Create()
    {
        var model = new StudentVm();
        return View(model);
    }

    //Create sutdent
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StudentVm model)
    {
        if (ModelState.IsValid)
        {
         
            var student = new Student
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
              //  RollNumber = nextRollNumber.ToString(), // assign numeric RollNumber
             //   IsActive = model.IsActive,
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }

    
    //Get Student for Edit
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var student = await _context.Students.FindAsync(id);
        if (student == null)
        {
            return NotFound();
        }

        var model = new StudentVm
        {
            StudentId = student.Id,
            Name = student.Name,
            Email = student.Email,
           Phone = student.Phone,
          //  IsActive = student.IsActive
        };
        return View(model);
    }
    
    //Post the change data Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, StudentVm model)
    {
        if (id != model.StudentId)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            student.Name = model.Name;
            student.Email = model.Email;
            student.Phone = model.Phone;
            student.RollNumber = model.RollNumber;
         //   student.IsActive = model.IsActive;  
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        return View(model);
    }
    
    //Get student for Delete 

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.Id == id);

        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }
    
    //Post for Delete 
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student != null)
        {
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool StudentExists(int id)
    {
        return _context.Students.Any(e => e.Id == id);
    }
}