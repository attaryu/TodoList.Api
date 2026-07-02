namespace TodoList.Api.Features.Todo.Core.DTOs.Inputs;

public record CreateTodoDto(string Title, string? Description, bool? IsCompleted);
