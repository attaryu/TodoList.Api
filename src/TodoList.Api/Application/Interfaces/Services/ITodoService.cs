using TodoList.Api.Application.DTOs.Todo.Inputs;
using TodoList.Api.Application.DTOs.Todo.Outputs;

namespace TodoList.Api.Application.Interfaces.Services;

public interface ITodoService
{
    Task<TodoResultDto> CreateAsync(CreateTodoDto dto, int userId);
    Task<TodoResultDto?> GetByIdAsync(Guid id, int userId);
    Task<IEnumerable<TodoResultDto>> GetAllByUserIdAsync(int userId);
    Task<TodoResultDto?> UpdateAsync(Guid id, UpdateTodoDto dto, int userId);
    Task<bool> DeleteAsync(Guid id, int userId);
    Task<TodoResultDto?> ToggleAsync(Guid id, int userId);
}
