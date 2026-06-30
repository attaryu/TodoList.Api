using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TodoList.Api.Features.Todo.Domain.Entities;

namespace TodoList.Api.Core.Infrastructure.Persistent;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TodoItem> TodoItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}