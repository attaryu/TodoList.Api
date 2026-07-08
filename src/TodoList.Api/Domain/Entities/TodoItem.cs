using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sindika.AspNet.Common.Interfaces;

namespace TodoList.Api.Domain.Entities;

[Table("dbs001_todoitem")]
public class TodoItem : IBaseEntity
{
    [Key]
    [Column("todoitem_id")]
    public Guid Id { get; set; }

    [Column("todoitem_title")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Column("todoitem_description")]
    [MaxLength(1000)]
    public string? Description { get; set; }

    [Column("todoitem_iscompleted")]
    [DefaultValue(false)]
    public bool IsCompleted { get; set; }

    [Column("todoitem_completedat")]
    public DateTimeOffset? CompletedAt { get; set; }

    [Column("todoitem_userid")]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [Column("todoitem_isactive")]
    public bool IsActive { get; set; }

    [Column("todoitem_createddate")]
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    [Column("todoitem_updateddate")]
    public DateTimeOffset? UpdatedDate { get; set; }

    [Column("todoitem_deleteddate")]
    public DateTimeOffset? DeletedDate { get; set; }

    [Column("todoitem_createdby")]
    public string? CreatedBy { get; set; }

    [Column("todoitem_updatedby")]
    public string? UpdatedBy { get; set; }

    [Column("todoitem_deletedby")]
    public string? DeletedBy { get; set; }
}
