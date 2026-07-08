using TodoList.Api.Application.Interfaces.Repositories;
using TodoList.Api.Domain.Entities;

namespace TodoList.Api.Application.Interfaces.Repositories;

public interface ITodoRepository : IBaseRepository<TodoItem>
{
    Task<IEnumerable<TodoItem>> GetAllByUserIdAsync(int userId);
    Task<TodoItem?> GetByIdAndUserIdAsync(Guid id, int userId);
}
