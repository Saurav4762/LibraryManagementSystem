// Data/LibraryDbContext.cs
using Microsoft.EntityFrameworkCore;
using Practice_Project.Entities; // ← Use Entities, not Models!

namespace Practice_Project.Data;

public class LibraryDbContext : DbContext
{
    public LibraryDbContext(DbContextOptions<LibraryDbContext> options) : base(options)
    {
    }

    // Correct: Only Entities here!
    public DbSet<User> Users { get; set; }
    public DbSet<Book> Books { get; set; } = null!;
    public DbSet<Author> Authors { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Student> Students { get; set; } = null!;
    public DbSet<BookIssue> BookIssues { get; set; } = null!;
    public DbSet<Fine> Fines { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Optional: Fine-tune relationships if needed
        modelBuilder.Entity<BookIssue>()
            .HasOne(bi => bi.Book)
            .WithMany(b => b.BookIssues)
            .HasForeignKey(bi => bi.BookId)
            .OnDelete(DeleteBehavior.Restrict);

        // === User ===
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Email).IsUnique();
            entity.Property(u => u.Email).HasMaxLength(256).IsRequired();
            entity.Property(u => u.Password).IsRequired();
            entity.Property(u => u.Role).HasMaxLength(50).IsRequired();
        });
        
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
    }
}