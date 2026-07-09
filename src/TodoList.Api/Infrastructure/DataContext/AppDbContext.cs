using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TodoList.Api.Domain.Entities;

namespace TodoList.Api.Infrastructure.DataContext;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public required DbSet<TodoItem> TodoItems { get; set; }
    public required DbSet<User> Users { get; set; }
    public required DbSet<EmailVerification> EmailVerifications { get; set; }
    public required DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TodoItem>().HasOne(e => e.User).WithMany().HasForeignKey(e => e.UserId);

        modelBuilder
            .Entity<EmailVerification>()
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder
            .Entity<PasswordResetToken>()
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
