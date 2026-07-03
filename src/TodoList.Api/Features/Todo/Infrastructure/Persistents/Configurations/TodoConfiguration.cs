using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoList.Api.Features.Todo.Core.Entities;

namespace TodoList.Api.Features.Todo.Infrastructure.Persistents.Configurations;

public class TodoConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
        entity.Property(e => e.Description).HasMaxLength(1000);
        entity.Property(e => e.IsCompleted).HasDefaultValue(false);
        entity.Property(e => e.CompletedAt).IsRequired(false);
        entity.Property(e => e.UserId).IsRequired();

        entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");

        entity
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
