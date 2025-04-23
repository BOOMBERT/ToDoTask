using Microsoft.EntityFrameworkCore;
using ToDoTask.Domain.Entities;
using ToDoTask.Infrastructure.Persistence.Configurations;

namespace ToDoTask.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<ToDoItem> ToDoItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new ToDoItemConfiguration());
    }
}