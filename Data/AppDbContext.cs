using Microsoft.EntityFrameworkCore;
using DogHouseService.Models;

namespace DogHouseService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Dog> Dogs { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dog>(eb =>
        {
            eb.HasKey(d => d.Id);
            eb.HasIndex(d => d.Name).IsUnique();
            eb.Property(d => d.Name).IsRequired();
            eb.Property(d => d.Color).IsRequired();
        });
    }
}