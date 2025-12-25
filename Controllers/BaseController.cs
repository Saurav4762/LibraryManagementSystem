using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Practice_Project.Controllers
{
    [Authorize] // This applies to all actions in controllers that inherit from this
    public abstract class BaseController : Controller
    {
        // Common functionality can go here
    }
}