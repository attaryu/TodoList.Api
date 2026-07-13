using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using TodoList.Api.Domain.Entities.Base;

namespace TodoList.Api.Domain.Entities;

[Table("dbs001_apikeys")]
[Index(nameof(KeyHash), IsUnique = true)]
public class ApiKey : IMetadata, ISoftDelete
{
    [Key]
    [Column("apikey_id")]
    public Guid Id { get; set; }

    [Column("apikey_keyhash")]
    [Required]
    [MaxLength(500)]
    public string KeyHash { get; set; } = string.Empty;

    [Column("apikey_prefix")]
    [Required]
    [MaxLength(20)]
    public string Prefix { get; set; } = string.Empty;

    [Column("apikey_label")]
    [Required]
    [MaxLength(50)]
    public string Label { get; set; } = string.Empty;

    [Column("apikey_userid")]
    [Required]
    public Guid UserId { get; set; }

    [Column("apikey_createddate")]
    [Required]
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    [Column("apikey_updateddate")]
    public DateTimeOffset? UpdatedDate { get; set; }

    [Column("apikey_isactive")]
    [Required]
    public bool IsActive { get; set; } = true;

    [Column("apikey_deleteddate")]
    public DateTimeOffset? DeletedDate { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;
}
