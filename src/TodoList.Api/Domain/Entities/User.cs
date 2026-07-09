using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TodoList.Api.Domain.Entities.Base;

namespace TodoList.Api.Domain.Entities;

[Table("dbs001_user")]
[Index(nameof(Email), IsUnique = true)]
public class User : IMetadata
{
    [Key]
    [Column("user_id")]
    public Guid Id { get; set; }

    [Column("user_fullname")]
    [MaxLength(50)]
    [Required]
    public string Fullname { get; set; } = string.Empty;

    [Column("user_email")]
    [MaxLength(256)]
    [Required]
    public string Email { get; set; } = string.Empty;

    [Column("user_passwordhash")]
    [MaxLength(500)]
    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("user_refreshtoken")]
    [MaxLength(500)]
    public string? RefreshToken { get; set; }

    [Column("user_refreshtokenexpiresat")]
    public DateTimeOffset? RefreshTokenExpiresAt { get; set; }

    [Column("user_isemailverified")]
    [DefaultValue(false)]
    public bool IsEmailVerified { get; set; } = false;

    [Column("user_createddate")]
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    [Column("user_updateddate")]
    public DateTimeOffset? UpdatedDate { get; set; }
}
