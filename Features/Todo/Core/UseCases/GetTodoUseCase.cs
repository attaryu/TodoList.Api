using TodoList.Api.Features.Todo.Core.Entities;
using TodoList.Api.Features.Todo.Core.Repositories;

namespace TodoList.Api.Features.Todo.Core.UseCases;

public class GetTodoUseCase(ITodoRepository todoRepository)
{
    private readonly ITodoRepository _todoRepository = todoRepository;

    public async Task<TodoItem?> ExecuteAsync(int id, int userId)
    {
        return await _todoRepository.GetByIdAndUserIdAsync(id, userId);
    }
}
