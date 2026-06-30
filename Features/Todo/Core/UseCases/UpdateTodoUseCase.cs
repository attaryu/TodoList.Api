using TodoList.Api.Features.Todo.Core.DTOs.Inputs;
using TodoList.Api.Features.Todo.Core.DTOs.Outputs;
using TodoList.Api.Features.Todo.Core.Repositories;
using TodoList.Api.Shared.Domain.Repositories;

namespace TodoList.Api.Features.Todo.Core.UseCases;

public class UpdateTodoUseCase(ITodoRepository todoRepository, IUnitOfWork unitOfWork)
{
    private readonly ITodoRepository _todoRepository = todoRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<TodoResultDto?> ExecuteAsync(int id, UpdateTodoDto dto, int userId)
    {
        var existingTodo = await _todoRepository.GetByIdAndUserIdAsync(id, userId);
        if (existingTodo == null)
        {
            return null;
        }

        existingTodo.Title = dto.Title;
        existingTodo.Description = dto.Description;
        existingTodo.IsCompleted = dto.IsCompleted;
        existingTodo.UpdatedAt = DateTime.UtcNow;
        existingTodo.CompletedAt = dto.IsCompleted
            ? existingTodo.CompletedAt ?? DateTime.UtcNow
            : null;

        _todoRepository.Update(existingTodo);
        await _unitOfWork.SaveChangesAsync();

        return new(
            existingTodo.Id,
            existingTodo.Title,
            existingTodo.Description,
            existingTodo.IsCompleted,
            existingTodo.CompletedAt,
            existingTodo.CreatedAt,
            existingTodo.UpdatedAt
        );
    }
}
