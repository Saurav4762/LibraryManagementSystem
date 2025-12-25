using Practice_Project.Data;
using Practice_Project.Models;
using System.Security.Cryptography;
using System.Text;

namespace Practice_Project.Data
{
    public static class DbInitializer
    {
        public static void Initialize(LibraryDbContext context)
        {
            context.Database.EnsureCreated();
            
            // Check if admin user exists
            if (!context.Users.Any())
            {
                // Create default admin user
                var admin = new User
                {
                    Email = "admin@library.com",
                    Password = HashPassword("Admin@123"),
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };
                
                context.Users.Add(admin);
                context.SaveChanges();
            }
        }
        
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }
}