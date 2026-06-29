using TodoList.Api.Core.Infrastructure.Persistent;
using TodoList.Api.Core.Infrastructure.Repositories;
using TodoList.Api.Features.Todo.Domain.Entities;
using TodoList.Api.Features.Todo.Domain.Repositories;

namespace TodoList.Api.Features.Todo.Infrastructure.Persistents.Repositories;

public class TodoRepositoryImpl(AppDbContext appDbContext) : BaseRepositoryImpl<TodoItem>(appDbContext), ITodoRepository
{
}
