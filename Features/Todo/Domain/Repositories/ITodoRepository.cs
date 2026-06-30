using TodoList.Api.Core.Domain.Repositories;
using TodoList.Api.Features.Todo.Domain.Entities;

namespace TodoList.Api.Features.Todo.Domain.Repositories;

public interface ITodoRepository : IBaseRepository<TodoItem>
{
}
