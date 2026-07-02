using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TodoList.Api.Features.Auth.Core.Entities;
using TodoList.Api.Features.Todo.Core.Entities;

namespace TodoList.Api.Shared.Infrastructure.Persistent;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public required DbSet<TodoItem> TodoItems { get; set; }
    public required DbSet<User> Users { get; set; }
    public required DbSet<EmailVerification> EmailVerifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
