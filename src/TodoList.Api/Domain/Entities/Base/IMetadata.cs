namespace TodoList.Api.Domain.Entities.Base;

public interface IMetadata
{
    public DateTimeOffset CreatedDate { get; set; }
    public DateTimeOffset? UpdatedDate { get; set; }
}
