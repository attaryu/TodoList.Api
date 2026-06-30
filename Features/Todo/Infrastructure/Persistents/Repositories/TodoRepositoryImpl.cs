using TodoList.Api.Shared.Infrastructure.Persistent;
using TodoList.Api.Shared.Infrastructure.Repositories;
using TodoList.Api.Features.Todo.Core.Entities;
using TodoList.Api.Features.Todo.Core.Repositories;

namespace TodoList.Api.Features.Todo.Infrastructure.Persistents.Repositories;

public class TodoRepositoryImpl(AppDbContext appDbContext) : BaseRepositoryImpl<TodoItem>(appDbContext), ITodoRepository
{
}
