using TodoList.Api.Features.Todo.Core.DTOs.Outputs;
using TodoList.Api.Features.Todo.Core.Repositories;

namespace TodoList.Api.Features.Todo.Core.UseCases;

public class GetTodosUseCase(ITodoRepository todoRepository)
{
    private readonly ITodoRepository _todoRepository = todoRepository;

    public async Task<IEnumerable<TodoResultDto>> ExecuteAsync(int userId)
    {
        var todos = await _todoRepository.GetAllByUserIdAsync(userId);

        return todos.Select(todo => new TodoResultDto(
            todo.Id,
            todo.Title,
            todo.Description,
            todo.IsCompleted,
            todo.CompletedAt,
            todo.CreatedAt,
            todo.UpdatedAt
        ));
    }
}
