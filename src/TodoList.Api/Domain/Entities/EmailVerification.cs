using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TodoList.Api.Domain.Entities;

[Table("dbs001_emailverification")]
[Index(nameof(Token), IsUnique = true)]
public class EmailVerification
{
    [Key]
    [Column("emailverification_id")]
    public Guid Id { get; set; }

    [Column("emailverification_userid")]
    [Required]
    public Guid UserId { get; set; }

    [Column("emailverification_token")]
    [MaxLength(256)]
    [Required]
    public string Token { get; set; } = string.Empty;

    [Column("emailverification_expiresat")]
    [Required]
    public DateTimeOffset ExpiresAt { get; set; }

    [Column("emailverification_createddate")]
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}
