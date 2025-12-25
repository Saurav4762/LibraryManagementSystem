using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Practice_Project.Data;
using Practice_Project.Entities;
using Practice_Project.Models;

namespace Practice_Project.Controllers;

public class StudentController : BaseController
{
    private readonly LibraryDbContext _context;

    public StudentController(LibraryDbContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        try
        {
            var students = await _context.Students
                .OrderBy(s => s.Name)
                .ToListAsync();
        
            //convert to ViewModel
            var studentsVms = students.Select(s=> new StudentVm
            {
                StudentId = s.StudentId,
                Name = s.Name,
                Email = s.Email,
                Phone = s.Phone,
                RollNumber = s.RollNumber,
                Semester = s.Semester,
                GeneratedStudentId = s.LoginId,
                IsActive = s.IsActive
            }).ToList();

            if (!students.Any())
            {
                ViewBag.Message = "No students found. Register a new student.";
            }
            
            return View(studentsVms);

        }
        catch (Exception e)
        {
            //Log Error
            Console.WriteLine($"Error in student Index: {e.Message}");
            
            //Return empty list instead of null 
            ViewBag.Message = "Error Loading students. Please try again.";
            return View(new List<StudentVm>());
        }
        
    }
  

    public async Task<IActionResult> Create()
    {
        var model = new StudentVm();
        return View(model);
    }

    //Student-Create
    [HttpPost]
    [ValidateAntiForgeryToken]

    [Authorize(Roles = "Admin,Librarian")]
    public async Task<IActionResult> Create(StudentVm model)
    {
        if (ModelState.IsValid)
        {
            var student = new Student
            {
                Name = model.Name,
                Email = model.Email,
                Phone = model.Phone,
                RollNumber = model.RollNumber,
                Semester = model.Semester,
                IsActive = model.IsActive,
            };
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }
    
    //Get Student for Edit\5
    [Authorize(Roles = "Admin,Librarian")]
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
            StudentId = student.StudentId,
            Name = student.Name,
            Email = student.Email,
            RollNumber = student.RollNumber,
            Semester = student.Semester,
            IsActive = student.IsActive
        };
        return View(model);
    }
    
    //Post the change data Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Librarian")]
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
            student.Semester = model.Semester;
            student.IsActive = model.IsActive;  
            
            _context.Update(student);
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
            .FirstOrDefaultAsync(s => s.StudentId == id);

        if (student == null)
        {
            return NotFound();
        }

        return View(student);
    }
    
    //Post for Delete 
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin,Librarian")]
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
        return _context.Students.Any(e => e.StudentId == id);
    }
}