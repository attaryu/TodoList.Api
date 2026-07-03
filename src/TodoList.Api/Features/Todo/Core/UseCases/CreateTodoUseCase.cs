using TodoList.Api.Features.Todo.Core.DTOs.Inputs;
using TodoList.Api.Features.Todo.Core.DTOs.Outputs;
using TodoList.Api.Features.Todo.Core.Entities;
using TodoList.Api.Features.Todo.Core.Repositories;
using TodoList.Api.Shared.Domain.Repositories;

namespace TodoList.Api.Features.Todo.Core.UseCases;

public class CreateTodoUseCase(ITodoRepository todoRepository, IUnitOfWork unitOfWork)
{
    private readonly ITodoRepository _todoRepository = todoRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<TodoResultDto> ExecuteAsync(CreateTodoDto dto, int userId)
    {
        bool isCompleted = dto.IsCompleted ?? false;

        TodoItem todo = new()
        {
            Title = dto.Title,
            Description = dto.Description,
            IsCompleted = isCompleted,
            UserId = userId,
            CompletedAt = isCompleted ? DateTime.UtcNow : null,
        };

        await _todoRepository.AddAsync(todo);
        await _unitOfWork.SaveChangesAsync();

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
