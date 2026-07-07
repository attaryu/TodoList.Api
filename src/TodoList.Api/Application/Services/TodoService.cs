using TodoList.Api.Application.DTOs.Todo.Inputs;
using TodoList.Api.Application.DTOs.Todo.Outputs;
using TodoList.Api.Application.Interfaces.Repositories;
using TodoList.Api.Application.Interfaces.Services;
using TodoList.Api.Domain.Entities;
using TodoList.Api.Shared.Domain.Repositories;

namespace TodoList.Api.Application.Services;

public class TodoService(ITodoRepository todoRepository, IUnitOfWork unitOfWork) : ITodoService
{
    private readonly ITodoRepository _todoRepository = todoRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<TodoResultDto> CreateAsync(CreateTodoDto dto, int userId)
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

    public async Task<TodoResultDto?> GetByIdAsync(int id, int userId)
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

    public async Task<IEnumerable<TodoResultDto>> GetAllByUserIdAsync(int userId)
    {
        var todos = await _todoRepository.GetAllByUserIdAsync(userId);

        return todos.Select(todo => new TodoResultDto(
            todo.Id,
            todo.Title,
            todo.Description,
            todo.IsCompleted,
            todo.CompletedAt,
            todo.CreatedAt,
            todo.UpdatedAt
        ));
    }

    public async Task<TodoResultDto?> UpdateAsync(int id, UpdateTodoDto dto, int userId)
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

    public async Task<bool> DeleteAsync(int id, int userId)
    {
        var todo = await _todoRepository.GetByIdAndUserIdAsync(id, userId);
        if (todo == null)
        {
            return false;
        }

        _todoRepository.Delete(todo);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<TodoResultDto?> ToggleAsync(int id, int userId)
    {
        var todo = await _todoRepository.GetByIdAndUserIdAsync(id, userId);
        if (todo == null)
        {
            return null;
        }

        todo.IsCompleted = !todo.IsCompleted;
        todo.CompletedAt = todo.IsCompleted ? DateTime.UtcNow : null;
        todo.UpdatedAt = DateTime.UtcNow;

        _todoRepository.Update(todo);
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
