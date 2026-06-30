using TodoList.Api.Shared.Domain.Repositories;
using TodoList.Api.Features.Todo.Core.Entities;

namespace TodoList.Api.Features.Todo.Core.Repositories;

public interface ITodoRepository : IBaseRepository<TodoItem>
{
    Task<IEnumerable<TodoItem>> GetAllByUserIdAsync(int userId);
    Task<TodoItem?> GetByIdAndUserIdAsync(int id, int userId);
}
