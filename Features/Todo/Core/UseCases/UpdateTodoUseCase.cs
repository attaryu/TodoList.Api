using TodoList.Api.Shared.Domain.Repositories;
using TodoList.Api.Features.Todo.Core.Entities;
using TodoList.Api.Features.Todo.Core.Repositories;

namespace TodoList.Api.Features.Todo.Core.UseCases;

public class UpdateTodoUseCase(ITodoRepository todoRepository, IUnitOfWork unitOfWork)
{
    private readonly ITodoRepository _todoRepository = todoRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<TodoItem?> ExecuteAsync(int id, TodoItem Todo)
    {
        var existingTodo = await _todoRepository.GetByIdAsync(id);
        if (existingTodo == null)
        {
            return null;
        }

        existingTodo.Title = Todo.Title;
        existingTodo.Description = Todo.Description;
        existingTodo.IsCompleted = Todo.IsCompleted;
        existingTodo.CompletedAt = Todo.IsCompleted ? existingTodo.CompletedAt ?? DateTime.UtcNow : null;
        existingTodo.UpdatedAt = DateTime.UtcNow;

        _todoRepository.Update(existingTodo);
        await _unitOfWork.SaveChangesAsync();

        return existingTodo;
    }
}
