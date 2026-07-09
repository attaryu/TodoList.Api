namespace TodoList.Api.Domain.Entities.Base;

public interface ISoftDelete
{
    public bool IsActive { get; set; }
    public DateTimeOffset? DeletedDate { get; set; }
}
