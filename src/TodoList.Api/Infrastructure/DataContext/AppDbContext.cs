using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TodoList.Api.Domain.Entities;

namespace TodoList.Api.Shared.Infrastructure.Persistent;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public required DbSet<TodoItem> TodoItems { get; set; }
    public required DbSet<User> Users { get; set; }
    public required DbSet<EmailVerification> EmailVerifications { get; set; }
    public required DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
