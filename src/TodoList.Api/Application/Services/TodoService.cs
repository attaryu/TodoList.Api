using TodoList.Api.Application.DTOs.Todo.Inputs;
using TodoList.Api.Application.DTOs.Todo.Outputs;
using TodoList.Api.Application.Interfaces.Repositories;
using TodoList.Api.Application.Interfaces.Services;
using TodoList.Api.Domain.Entities;

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
            Id: todo.Id,
            Title: todo.Title,
            Description: todo.Description,
            IsCompleted: todo.IsCompleted,
            CompletedAt: todo.CompletedAt,
            CreatedDate: todo.CreatedDate,
            UpdatedDate: todo.UpdatedDate
        );
    }

    public async Task<TodoResultDto?> GetByIdAsync(Guid id, int userId)
    {
        var todo = await _todoRepository.GetByIdAndUserIdAsync(id, userId);
        if (todo == null)
        {
            return null;
        }

        return new(
            Id: todo.Id,
            Title: todo.Title,
            Description: todo.Description,
            IsCompleted: todo.IsCompleted,
            CompletedAt: todo.CompletedAt,
            CreatedDate: todo.CreatedDate,
            UpdatedDate: todo.UpdatedDate
        );
    }

    public async Task<IEnumerable<TodoResultDto>> GetAllByUserIdAsync(int userId)
    {
        var todos = await _todoRepository.GetAllByUserIdAsync(userId);

        return todos.Select(todo => new TodoResultDto(
            Id: todo.Id,
            Title: todo.Title,
            Description: todo.Description,
            IsCompleted: todo.IsCompleted,
            CompletedAt: todo.CompletedAt,
            CreatedDate: todo.CreatedDate,
            UpdatedDate: todo.UpdatedDate
        ));
    }

    public async Task<TodoResultDto?> UpdateAsync(Guid id, UpdateTodoDto dto, int userId)
    {
        var existingTodo = await _todoRepository.GetByIdAndUserIdAsync(id, userId);
        if (existingTodo == null)
        {
            return null;
        }

        existingTodo.Title = dto.Title;
        existingTodo.Description = dto.Description;
        existingTodo.IsCompleted = dto.IsCompleted;
        existingTodo.UpdatedDate = DateTime.UtcNow;
        existingTodo.CompletedAt = dto.IsCompleted
            ? existingTodo.CompletedAt ?? DateTime.UtcNow
            : null;

        _todoRepository.Update(existingTodo);
        await _unitOfWork.SaveChangesAsync();

        return new(
            Id: existingTodo.Id,
            Title: existingTodo.Title,
            Description: existingTodo.Description,
            IsCompleted: existingTodo.IsCompleted,
            CompletedAt: existingTodo.CompletedAt,
            CreatedDate: existingTodo.CreatedDate,
            UpdatedDate: existingTodo.UpdatedDate
        );
    }

    public async Task<bool> DeleteAsync(Guid id, int userId)
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

    public async Task<TodoResultDto?> ToggleAsync(Guid id, int userId)
    {
        var todo = await _todoRepository.GetByIdAndUserIdAsync(id, userId);
        if (todo == null)
        {
            return null;
        }

        todo.IsCompleted = !todo.IsCompleted;
        todo.CompletedAt = todo.IsCompleted ? DateTime.UtcNow : null;
        todo.UpdatedDate = DateTime.UtcNow;

        _todoRepository.Update(todo);
        await _unitOfWork.SaveChangesAsync();

        return new(
            Id: todo.Id,
            Title: todo.Title,
            Description: todo.Description,
            IsCompleted: todo.IsCompleted,
            CompletedAt: todo.CompletedAt,
            CreatedDate: todo.CreatedDate,
            UpdatedDate: todo.UpdatedDate
        );
    }
}
