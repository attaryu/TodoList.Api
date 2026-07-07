using TodoList.Api.Domain.Entities;
using TodoList.Api.Shared.Domain.Repositories;

namespace TodoList.Api.Application.Interfaces.Repositories;

public interface ITodoRepository : IBaseRepository<TodoItem>
{
    Task<IEnumerable<TodoItem>> GetAllByUserIdAsync(int userId);
    Task<TodoItem?> GetByIdAndUserIdAsync(int id, int userId);
}
