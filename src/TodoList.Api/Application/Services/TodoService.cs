using Mapster;
using Sindika.AspNet.Exceptions.NotFound;
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

        return todo.Adapt<TodoResultDto>();
    }

    public async Task<TodoResultDto> GetByIdAsync(Guid id, int userId)
    {
        var todo =
            await _todoRepository.GetByIdAndUserIdAsync(id, userId)
            ?? throw new NotFoundException($"No Todo Item with ID {id}");

        return todo.Adapt<TodoResultDto>();
    }

    public async Task<IEnumerable<TodoResultDto>> GetAllByUserIdAsync(int userId)
    {
        var todos = await _todoRepository.GetAllByUserIdAsync(userId);
        return todos.Adapt<IEnumerable<TodoResultDto>>();
    }

    public async Task<TodoResultDto> UpdateAsync(Guid id, UpdateTodoDto dto, int userId)
    {
        var existingTodo =
            await _todoRepository.GetByIdAndUserIdAsync(id, userId)
            ?? throw new NotFoundException($"No Todo Item with ID {id}");

        existingTodo.Title = dto.Title;
        existingTodo.Description = dto.Description;
        existingTodo.IsCompleted = dto.IsCompleted;
        existingTodo.UpdatedDate = DateTime.UtcNow;
        existingTodo.CompletedAt = dto.IsCompleted
            ? existingTodo.CompletedAt ?? DateTime.UtcNow
            : null;

        _todoRepository.Update(existingTodo);
        await _unitOfWork.SaveChangesAsync();

        return existingTodo.Adapt<TodoResultDto>();
    }

    public async Task<bool> DeleteAsync(Guid id, int userId)
    {
        var todo = await _todoRepository.GetByIdAndUserIdAsync(id, userId);
        if (todo == null)
        {
            return true;
        }

        _todoRepository.Delete(todo);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<TodoResultDto> ToggleAsync(Guid id, int userId)
    {
        var todo =
            await _todoRepository.GetByIdAndUserIdAsync(id, userId)
            ?? throw new NotFoundException($"No Todo Item with ID {id}");

        todo.IsCompleted = !todo.IsCompleted;
        todo.CompletedAt = todo.IsCompleted ? DateTime.UtcNow : null;
        todo.UpdatedDate = DateTime.UtcNow;

        _todoRepository.Update(todo);
        await _unitOfWork.SaveChangesAsync();

        return todo.Adapt<TodoResultDto>();
    }
}
