using Microsoft.AspNetCore.Mvc.Rendering;

namespace Practice_Project.Models;

public class BookViewModel
{
    public Book Book { get; set; }
    
    public IEnumerable<SelectListItem> Authors { get; set; }
    
    public IEnumerable<SelectListItem> Categories { get; set; }
}