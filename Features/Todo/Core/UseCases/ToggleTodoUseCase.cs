using TodoList.Api.Features.Todo.Core.DTOs.Outputs;
using TodoList.Api.Features.Todo.Core.Repositories;
using TodoList.Api.Shared.Domain.Repositories;

namespace TodoList.Api.Features.Todo.Core.UseCases;

public class ToggleTodoUseCase(ITodoRepository todoRepository, IUnitOfWork unitOfWork)
{
    private readonly ITodoRepository _todoRepository = todoRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<TodoResultDto?> ExecuteAsync(int id, int userId)
    {
        var Todo = await _todoRepository.GetByIdAndUserIdAsync(id, userId);
        if (Todo == null)
        {
            return null;
        }

        Todo.IsCompleted = !Todo.IsCompleted;
        Todo.CompletedAt = Todo.IsCompleted ? DateTime.UtcNow : null;
        Todo.UpdatedAt = DateTime.UtcNow;

        _todoRepository.Update(Todo);
        await _unitOfWork.SaveChangesAsync();

        return new(
            Todo.Id,
            Todo.Title,
            Todo.Description,
            Todo.IsCompleted,
            Todo.CompletedAt,
            Todo.CreatedAt,
            Todo.UpdatedAt
        );
    }
}
