using TodoList.Api.Core.Domain.Repositories;
using TodoList.Api.Features.Todo.Domain.Entities;
using TodoList.Api.Features.Todo.Domain.Repositories;

namespace TodoList.Api.Features.Todo.Domain.UseCases;

public class UpdateTodoItemUseCase(ITodoRepository todoRepository, IUnitOfWork unitOfWork)
{
    private readonly ITodoRepository _todoRepository = todoRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<TodoItem?> ExecuteAsync(int id, TodoItem todoItem)
    {
        var existingTodoItem = await _todoRepository.GetByIdAsync(id);
        if (existingTodoItem == null)
        {
            return null;
        }

        existingTodoItem.Title = todoItem.Title;
        existingTodoItem.Description = todoItem.Description;
        existingTodoItem.IsCompleted = todoItem.IsCompleted;
        existingTodoItem.CompletedAt = todoItem.IsCompleted ? DateTime.UtcNow : null;
        existingTodoItem.UpdatedAt = DateTime.UtcNow;

        _todoRepository.Update(existingTodoItem);
        await _unitOfWork.SaveChangesAsync();

        return existingTodoItem;
    }
}
