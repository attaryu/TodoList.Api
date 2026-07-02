using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoList.Api.Features.Auth.Core.Entities;

namespace TodoList.Api.Features.Auth.Infrastructure.Persistents.Configurations;

public class EmailVerificationConfiguration : IEntityTypeConfiguration<EmailVerification>
{
    public void Configure(EntityTypeBuilder<EmailVerification> entity)
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
