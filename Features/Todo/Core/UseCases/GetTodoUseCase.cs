using TodoList.Api.Features.Todo.Core.DTOs.Outputs;
using TodoList.Api.Features.Todo.Core.Repositories;

namespace TodoList.Api.Features.Todo.Core.UseCases;

public class GetTodoUseCase(ITodoRepository todoRepository)
{
    private readonly ITodoRepository _todoRepository = todoRepository;

    public async Task<TodoResultDto?> ExecuteAsync(int id, int userId)
    {
        var todo = await _todoRepository.GetByIdAndUserIdAsync(id, userId);
        if (todo == null)
        {
            return null;
        }

        return new(
            todo.Id,
            todo.Title,
            todo.Description,
            todo.IsCompleted,
            todo.CompletedAt,
            todo.CreatedAt,
            todo.UpdatedAt
        );
    }
}
