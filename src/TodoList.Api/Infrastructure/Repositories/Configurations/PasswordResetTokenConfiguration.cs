using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoList.Api.Domain.Entities;

namespace TodoList.Api.Infrastructure.Repositories.Configurations;

public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
{
    public void Configure(EntityTypeBuilder<PasswordResetToken> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Token).IsRequired().HasMaxLength(256);
        entity.HasIndex(e => e.Token).IsUnique();

        entity.Property(e => e.UserId).IsRequired();
        entity.Property(e => e.ExpiresAt).IsRequired();
        entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");

        entity
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
