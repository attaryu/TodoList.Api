using TodoList.Api.Core.Domain.Repositories;
using TodoList.Api.Features.Todo.Domain.Entities;
using TodoList.Api.Features.Todo.Domain.Repositories;

namespace TodoList.Api.Features.Todo.Domain.UseCases;

public class ToggleTodoUseCase(ITodoRepository todoRepository, IUnitOfWork unitOfWork)
{
    private readonly ITodoRepository _todoRepository = todoRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<TodoItem?> ExecuteAsync(int id)
    {
        var Todo = await _todoRepository.GetByIdAsync(id);
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
