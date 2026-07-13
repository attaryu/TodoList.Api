using TodoList.Api.Application.DTOs.ApiKey.Inputs;
using TodoList.Api.Application.DTOs.ApiKey.Outputs;

namespace TodoList.Api.Application.Interfaces.Services;

public interface IApiKeyService
{
    Task<ApiKeyCreatedResultDto> CreateAsync(CreateApiKeyDto dto, Guid userId);
    Task<IEnumerable<ApiKeyResultDto>> GetAllByUserIdAsync(Guid userId);
    Task<bool> RevokeAsync(Guid id, Guid userId);
    Task<ApiKeyResultDto> UpdateLabelAsync(Guid id, UpdateApiKeyDto dto, Guid userId);
    Task<AuthenticatedApiKeyResult?> VerifyApiKeyAsync(string rawKey);
}
