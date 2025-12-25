using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Practice_Project.Data;
using Practice_Project.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Practice_Project.Controllers
{
    [AllowAnonymous]
    public class AuthController : BaseController
    {
        private readonly LibraryDbContext _context;
        
        public AuthController(LibraryDbContext context)
        {
            _context = context;
        }
        
        // GET: /Auth/Login
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");
                
            return View();
        }
        
        // POST: /Auth/Login
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Please enter email and password";
                return View();
            }
            
            // Check if user exists
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.IsActive);
                
            if (user == null || !VerifyPassword(password, user.Password))
            {
                ViewBag.Error = "Invalid email or password";
                return View();
            }
            
            // Create claims - CORRECT VERSION
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), // Convert int to string
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.Id.ToString()) // Convert int to string
            };
            
            var claimsIdentity = new ClaimsIdentity(claims, 
                CookieAuthenticationDefaults.AuthenticationScheme);
            
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(5)
            };
            
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
            
            return RedirectToAction("Index", "Home");
        }
       
        public IActionResult AccessDenied()
        {
            return View();
        }
        
        
        // GET: /Auth/StudentLogin
        public IActionResult StudentLogin()
        {
            return View();
        }
        
        // POST: /Auth/StudentLogin - CORRECTED VERSION
        [HttpPost]
        public async Task<IActionResult> StudentLogin(string loginId, string password)
        {
            if (string.IsNullOrEmpty(loginId) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Please enter Login ID and password";
                return View();
            }
            
            // Find student by LoginId
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.LoginId == loginId && s.IsActive);
                
            if (student == null || !VerifyPassword(password, student.Password))
            {
                ViewBag.Error = "Invalid Login ID or password";
                return View();
            }
            
            // Create claims - CORRECT: Convert int to string
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, student.StudentId.ToString()), // int to string
                new Claim(ClaimTypes.Name, student.Name),
                new Claim(ClaimTypes.Role, "Student"),
                new Claim("StudentId", student.StudentId.ToString()), // int to string
                new Claim("StudentName", student.Name),
                new Claim("StudentLoginId", student.LoginId ?? "")
            };
            
            var claimsIdentity = new ClaimsIdentity(claims, 
                CookieAuthenticationDefaults.AuthenticationScheme);
            
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
            };
            
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
            
            return RedirectToAction("StudentDashboard", "Home");
        }
        
        // GET: /Auth/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        
        
        
          // ===== REGISTER METHODS =====
        
        // GET: /Auth/Register
        [HttpGet]
        public IActionResult Register()
        {
            // Check if user is already logged in
            if (User.Identity.IsAuthenticated)
            {
                // If already logged in, check if user is admin
                if (User.IsInRole("Admin"))
                {
                    return View();
                }
                else
                {
                    // Redirect non-admin users to home
                    return RedirectToAction("Index", "Home");
                }
            }
            
            // For registration, you might want to allow only admins to register new users
            // If you want to allow anyone to register, remove the admin check
            return View();
        }
        
        // POST: /Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email already registered.");
                return View(model);
            }
            
            // Create new user
            var user = new User
            {
                Email = model.Email,
                Password = HashPassword(model.Password),
                Role = "Librarian", // Default role, or you can make this selectable
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            // Option 1: Auto login after registration
            // await SignInUser(user);
            // return RedirectToAction("Index", "Home");
            
            // Option 2: Redirect to login page
            TempData["SuccessMessage"] = "Registration successful! Please login.";
            return RedirectToAction("Login");
        }
        
        // Alternative: Register with admin authorization only
        // GET: /Auth/RegisterAdminOnly
        [HttpGet]
        [Authorize(Roles = "Admin")] // Only admin can access this
        public IActionResult RegisterAdminOnly()
        {
            return View();
        }
        
        // POST: /Auth/RegisterAdminOnly
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAdminOnly(UserRegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            // Check if email already exists
            if (await _context.Users.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "Email already registered.");
                return View(model);
            }
            
            var user = new User
            {
                Email = model.Email,
                Password = HashPassword(model.Password),
                Role = model.Role, // Role selected in form
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            
            TempData["SuccessMessage"] = $"New {model.Role} registered successfully!";
            return RedirectToAction("Index", "Home");
        }
        
        // Helper method for auto login after registration
        private async Task SignInUser(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("UserId", user.Id.ToString())
            };
            
            var claimsIdentity = new ClaimsIdentity(claims, 
                CookieAuthenticationDefaults.AuthenticationScheme);
            
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2)
            };
            
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }
        
        // ... [Your existing HashPassword and VerifyPassword methods] ...
        
        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
        
        private bool VerifyPassword(string password, string hashedPassword)
        {
            var hash = HashPassword(password);
            return hash == hashedPassword;
        }
    }
}

