using TodoList.Api.Shared.Domain.Repositories;
using TodoList.Api.Features.Todo.Core.Entities;
using TodoList.Api.Features.Todo.Core.Repositories;

namespace TodoList.Api.Features.Todo.Core.UseCases;

public class ToggleTodoUseCase(ITodoRepository todoRepository, IUnitOfWork unitOfWork)
{
    private readonly ITodoRepository _todoRepository = todoRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<TodoItem?> ExecuteAsync(int id, int userId)
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

        return Todo;
    }
}
