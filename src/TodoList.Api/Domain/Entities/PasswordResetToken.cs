using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TodoList.Api.Domain.Entities;

[Table("dbs001_passwordresettoken")]
[Index(nameof(Token), IsUnique = true)]
public class PasswordResetToken
{
    [Key]
    [Column("passwordresettoken_id")]
    public Guid Id { get; set; }

    [Column("passwordresettoken_userid")]
    [Required]
    public Guid UserId { get; set; }

    [Column("passwordresettoken_token")]
    [MaxLength(256)]
    [Required]
    public string Token { get; set; } = string.Empty;

    [Column("passwordresettoken_expiresat")]
    [Required]
    public DateTimeOffset ExpiresAt { get; set; }

    [Column("passwordresettoken_createddate")]
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}
