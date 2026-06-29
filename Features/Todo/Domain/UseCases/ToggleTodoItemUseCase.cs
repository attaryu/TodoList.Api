using TodoList.Api.Core.Domain.Repositories;
using TodoList.Api.Features.Todo.Domain.Entities;
using TodoList.Api.Features.Todo.Domain.Repositories;

namespace TodoList.Api.Features.Todo.Domain.UseCases;

public class ToggleTodoItemUseCase(ITodoRepository todoRepository, IUnitOfWork unitOfWork)
{
    private readonly ITodoRepository _todoRepository = todoRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<TodoItem?> ExecuteAsync(int id)
    {
        var todoItem = await _todoRepository.GetByIdAsync(id);
        if (todoItem == null)
        {
            return null;
        }

        todoItem.IsCompleted = !todoItem.IsCompleted;
        todoItem.CompletedAt = todoItem.IsCompleted ? DateTime.UtcNow : null;
        todoItem.UpdatedAt = DateTime.UtcNow;

        _todoRepository.Update(todoItem);
        await _unitOfWork.SaveChangesAsync();

        return todoItem;
    }
}
