namespace TodoList.Api.Application.DTOs.Todo.Outputs;

public record TodoResultDto(
    Guid Id,
    string Title,
    string? Description,
    bool IsCompleted,
    DateTimeOffset? CompletedDate,
    DateTimeOffset CreatedDate,
    DateTimeOffset? UpdatedDate
);
