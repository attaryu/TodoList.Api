using TodoList.Api.Application.DTOs.Todo.Inputs;
using TodoList.Api.Application.DTOs.Todo.Outputs;

namespace TodoList.Api.Application.Interfaces.Services;

public interface ITodoService
{
    Task<TodoResultDto> CreateAsync(CreateTodoDto dto, int userId);
    Task<TodoResultDto?> GetByIdAsync(int id, int userId);
    Task<IEnumerable<TodoResultDto>> GetAllByUserIdAsync(int userId);
    Task<TodoResultDto?> UpdateAsync(int id, UpdateTodoDto dto, int userId);
    Task<bool> DeleteAsync(int id, int userId);
    Task<TodoResultDto?> ToggleAsync(int id, int userId);
}
