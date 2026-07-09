using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TodoList.Api.Domain.Entities.Base;

namespace TodoList.Api.Domain.Entities;

[Table("dbs001_todoitem")]
public class TodoItem : ISoftDelete, IMetadata
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

    [Column("todoitem_completedDate")]
    public DateTimeOffset? CompletedDate { get; set; }

    [Column("todoitem_userid")]
    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = null!;

    [Column("todoitem_isactive")]
    public bool IsActive { get; set; } = true;

    [Column("todoitem_createddate")]
    public DateTimeOffset CreatedDate { get; set; } = DateTimeOffset.UtcNow;

    [Column("todoitem_updateddate")]
    public DateTimeOffset? UpdatedDate { get; set; }

    [Column("todoitem_deleteddate")]
    public DateTimeOffset? DeletedDate { get; set; }
}
