namespace TodoList.Api.Application.DTOs.ApiKey.Outputs;

public record AuthenticatedApiKeyResult(Guid UserId, Guid ApiKeyId);
