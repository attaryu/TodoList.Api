using TodoList.Api.Domain.Entities;

namespace TodoList.Api.Application.Interfaces.Repositories;

public interface ITodoRepository : IBaseRepository<TodoItem>
{
    Task<IEnumerable<TodoItem>> GetAllByUserIdAsync(Guid userId);
    Task<TodoItem?> GetByIdAndUserIdAsync(Guid id, Guid userId);
    Task<(IEnumerable<TodoItem> Items, int TotalCount)> GetPagedByUserIdAsync(
        Guid userId,
        int page,
        int limit
    );
}
