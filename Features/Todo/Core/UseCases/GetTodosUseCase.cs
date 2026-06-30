using TodoList.Api.Features.Todo.Core.Entities;
using TodoList.Api.Features.Todo.Core.Repositories;

namespace TodoList.Api.Features.Todo.Core.UseCases;

public class GetTodosUseCase(ITodoRepository todoRepository)
{
    private readonly ITodoRepository _todoRepository = todoRepository;

    public async Task<IEnumerable<TodoItem>> ExecuteAsync(int userId)
    {
        return await _todoRepository.GetAllByUserIdAsync(userId);
    }
}
