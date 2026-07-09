using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using TodoList.Api.Domain.Entities;
using TodoList.Api.Domain.Entities.Base;

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

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder
                    .Entity(entityType.ClrType)
                    .HasQueryFilter(ConvertFilterExpression(entityType.ClrType));
            }
        }
    }

    public override int SaveChanges()
    {
        ApplySoftDelete();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplySoftDelete();
        return base.SaveChangesAsync(cancellationToken);
    }

    private void ApplySoftDelete()
    {
        foreach (var entry in ChangeTracker.Entries<ISoftDelete>())
        {
            if (entry.State == EntityState.Deleted)
            {
                entry.State = EntityState.Modified;
                entry.Entity.IsActive = false;
                entry.Entity.DeletedDate = DateTimeOffset.UtcNow;
            }
        }
    }

    private static LambdaExpression ConvertFilterExpression(Type type)
    {
        var parameter = Expression.Parameter(type, "e");
        var property = Expression.Property(parameter, nameof(ISoftDelete.IsActive));
        var body = Expression.Equal(property, Expression.Constant(true));

        return Expression.Lambda(body, parameter);
    }
}
