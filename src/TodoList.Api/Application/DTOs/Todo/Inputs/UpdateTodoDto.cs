namespace TodoList.Api.Application.DTOs.Todo.Inputs;

public record UpdateTodoDto(string Title, string Description, bool IsCompleted);
