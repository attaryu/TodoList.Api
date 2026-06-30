namespace TodoList.Api.Features.Todo.Core.DTOs.Outputs;

public record TodoResultDto(
    int Id,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTime? CompletedAt,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
