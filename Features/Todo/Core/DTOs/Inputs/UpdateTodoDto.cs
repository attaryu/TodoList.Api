namespace TodoList.Api.Features.Todo.Core.DTOs.Inputs;

public record UpdateTodoDto(string Title, string Description, bool IsCompleted);
