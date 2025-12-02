using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Practice_Project.Data;
using Practice_Project.Models;

namespace Practice_Project.Controllers;

public class BookController : Controller

{
    
    // depandency ( interact with database)  making object
    private readonly LibraryDbContext _context;
    public BookController(LibraryDbContext context)
    {
        _context = context;
    }
    
    public IActionResult Create()
    {
        // Adding the BookViewModel.cs in this 
        var viewModel = new BookViewModel();
        viewModel.Categories = _context.Categories.Select(c=>
            new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.Name
            })
                .ToList();
            
        return View(viewModel);
    }
}