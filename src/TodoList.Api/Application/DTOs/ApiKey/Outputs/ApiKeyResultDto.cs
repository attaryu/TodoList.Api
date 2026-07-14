namespace TodoList.Api.Application.DTOs.ApiKey.Outputs;

public record ApiKeyResultDto(
    Guid Id,
    string Label,
    string MaskedKey,
    DateTimeOffset CreatedDate,
    DateTimeOffset? UpdatedDate
);
