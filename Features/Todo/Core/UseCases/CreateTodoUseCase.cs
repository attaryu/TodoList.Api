using TodoList.Api.Shared.Domain.Repositories;
using TodoList.Api.Features.Todo.Core.Entities;
using TodoList.Api.Features.Todo.Core.Repositories;

namespace TodoList.Api.Features.Todo.Core.UseCases;

public class CreateTodoUseCase(ITodoRepository todoRepository, IUnitOfWork unitOfWork)
{
    private readonly ITodoRepository _todoRepository = todoRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<TodoItem> ExecuteAsync(TodoItem todo, int userId)
    {
        if (string.IsNullOrWhiteSpace(todo.Title))
        {
            throw new ArgumentException("Title is required");
        }

        todo.UserId = userId;
        todo.CompletedAt = todo.IsCompleted ? DateTime.UtcNow : null;

        await _todoRepository.AddAsync(todo);
        await _unitOfWork.SaveChangesAsync();

        return todo;
    }
}
