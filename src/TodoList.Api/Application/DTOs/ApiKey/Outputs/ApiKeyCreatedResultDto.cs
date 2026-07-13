namespace TodoList.Api.Application.DTOs.ApiKey.Outputs;

public record ApiKeyCreatedResultDto(
    Guid Id,
    string Label,
    string Key,
    DateTimeOffset CreatedDate,
    DateTimeOffset? UpdatedDate
);
