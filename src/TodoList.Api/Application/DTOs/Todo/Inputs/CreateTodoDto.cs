namespace TodoList.Api.Application.DTOs.Todo.Inputs;

public record CreateTodoDto(string Title, string? Description, bool? IsCompleted);
