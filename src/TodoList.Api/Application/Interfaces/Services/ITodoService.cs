using TodoList.Api.Application.DTOs.Todo.Inputs;
using TodoList.Api.Application.DTOs.Todo.Outputs;

namespace TodoList.Api.Application.Interfaces.Services;

public interface ITodoService
{
    Task<TodoResultDto> CreateAsync(CreateTodoDto dto, Guid userId);
    Task<TodoResultDto> GetByIdAsync(Guid id, Guid userId);
    Task<IEnumerable<TodoResultDto>> GetAllByUserIdAsync(Guid userId);
    Task<TodoResultDto> UpdateAsync(Guid id, UpdateTodoDto dto, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
    Task<TodoResultDto> ToggleAsync(Guid id, Guid userId);
}
