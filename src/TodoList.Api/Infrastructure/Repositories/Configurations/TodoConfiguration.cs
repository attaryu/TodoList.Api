using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoList.Api.Domain.Entities;

namespace TodoList.Api.Infrastructure.Repositories.Configurations;

public class TodoConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Description).HasMaxLength(1000);
        builder.Property(e => e.IsCompleted).HasDefaultValue(false);
        builder.Property(e => e.CompletedAt).IsRequired(false);
        builder.Property(e => e.UserId).IsRequired();

        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");

        builder
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
