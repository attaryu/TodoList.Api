using TodoList.Api.Shared.Domain.Repositories;
using TodoList.Api.Features.Todo.Core.Entities;

namespace TodoList.Api.Features.Todo.Core.Repositories;

public interface ITodoRepository : IBaseRepository<TodoItem>
{
}
