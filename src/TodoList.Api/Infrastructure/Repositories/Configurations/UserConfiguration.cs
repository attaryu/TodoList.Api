using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoList.Api.Domain.Entities;

namespace TodoList.Api.Features.Auth.Infrastructure.Persistents.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Fullname).IsRequired().HasMaxLength(50);
        entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
        entity.HasIndex(e => e.Email).IsUnique();

        entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
        entity.Property(e => e.RefreshToken).HasMaxLength(500).IsRequired(false);
        entity.Property(e => e.RefreshTokenExpiresAt).IsRequired(false);
        entity.Property(e => e.IsEmailVerified).HasDefaultValue(false);

        entity.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
        entity.Property(e => e.UpdatedAt).HasDefaultValueSql("NOW()");
    }
}
