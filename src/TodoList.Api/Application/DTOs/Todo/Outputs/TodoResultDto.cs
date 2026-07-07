namespace TodoList.Api.Application.DTOs.Todo.Outputs;

public record TodoResultDto(
    int Id,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTime? CompletedAt,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
