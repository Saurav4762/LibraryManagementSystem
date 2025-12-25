// Data/LibraryDbContext.cs
using Microsoft.EntityFrameworkCore;
using Practice_Project.Entities;
using Practice_Project.Models; // ← Use Entities, not Models!

namespace Practice_Project.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
    {
    }

    // Correct: Only Entities here!
    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<Author> Authors { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Student> Students { get; set; } = null!;
    public DbSet<BookIssue> BookIssues { get; set; } = null!;
    
    public DbSet<Fine> Fines { get; set; } = null!;
    
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Optional: Fine-tune relationships if needed
        modelBuilder.Entity<BookIssue>()
            .HasOne(bi => bi.Book)
            .WithMany(b => b.BookIssues)
            .HasForeignKey(bi => bi.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<BookIssue>()
            .HasOne(bi => bi.Student)
            .WithMany(s => s.BookIssues)
            .HasForeignKey(bi => bi.StudentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Fine>()
            .HasOne(f => f.BookIssue)
            .WithMany()
            .HasForeignKey(f => f.BookIssueId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
            
        // Configure User entity
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
                
        // Configure Student entity
        modelBuilder.Entity<Student>()
            .HasIndex(s => s.Email)
            .IsUnique();
                
        modelBuilder.Entity<Student>()
            .HasIndex(s => s.StudentId)
            .IsUnique();
    }
}