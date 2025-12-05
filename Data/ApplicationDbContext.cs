using Microsoft.EntityFrameworkCore;
using MyNetflixClone.Models;

namespace MyNetflixClone.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Movie> Movies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired();
            entity.Property(e => e.Slug).IsRequired();
            entity.Property(e => e.FilePath).IsRequired();
            entity.HasIndex(e => e.Title);
            entity.HasIndex(e => e.Slug).IsUnique();
            entity.HasIndex(e => e.Genre);
            entity.HasIndex(e => e.Year);
        });
    }
}
