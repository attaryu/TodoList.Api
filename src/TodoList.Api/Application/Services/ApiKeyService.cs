using System.Security.Cryptography;
using Mapster;
using Sindika.AspNet.Exceptions.BadRequest;
using Sindika.AspNet.Exceptions.NotFound;
using TodoList.Api.Application.DTOs.ApiKey.Inputs;
using TodoList.Api.Application.DTOs.ApiKey.Outputs;
using TodoList.Api.Application.Interfaces.Repositories;
using TodoList.Api.Application.Interfaces.Services;
using TodoList.Api.Domain.Entities;

namespace TodoList.Api.Application.Services;

public class ApiKeyService(
    IApiKeyRepository apiKeyRepository,
    IUnitOfWork unitOfWork,
    IHasherProvider hasherProvider
) : IApiKeyService
{
    private readonly IApiKeyRepository _apiKeyRepository = apiKeyRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IHasherProvider _hasherProvider = hasherProvider;

    public async Task<ApiKeyCreatedResultDto> CreateAsync(CreateApiKeyDto dto, Guid userId)
    {
        var count = await _apiKeyRepository.CountActiveByUserIdAsync(userId);
        if (count >= 3)
        {
            throw new InvalidOperationException("Maximum 3 active API keys allowed.");
        }

        var randomBytes = new byte[32];
        RandomNumberGenerator.Fill(randomBytes);

        var rawKeySuffix = Convert.ToHexString(randomBytes).ToLowerInvariant();
        var rawKey = $"tlk_{rawKeySuffix}";
        var prefix = rawKey[..12];
        var keyHash = _hasherProvider.HashText(rawKey);

        var apiKey = new ApiKey
        {
            Label = dto.Label,
            KeyHash = keyHash,
            Prefix = prefix,
            UserId = userId,
            IsActive = true,
        };

        await _apiKeyRepository.AddAsync(apiKey);
        await _unitOfWork.SaveChangesAsync();

        return new(
            Id: apiKey.Id,
            Label: apiKey.Label,
            Key: rawKey,
            CreatedDate: apiKey.CreatedDate,
            UpdatedDate: apiKey.UpdatedDate
        );
    }

    public async Task<IEnumerable<ApiKeyResultDto>> GetAllByUserIdAsync(Guid userId)
    {
        var apiKeys = await _apiKeyRepository.GetAllByUserIdAsync(userId);

        return apiKeys.Select(k => new ApiKeyResultDto(
            Id: k.Id,
            Label: k.Label,
            MaskedKey: $"{k.Prefix}••••••••••",
            CreatedDate: k.CreatedDate,
            UpdatedDate: k.UpdatedDate
        ));
    }

    public async Task<bool> RevokeAsync(Guid id, Guid userId)
    {
        var apiKey =
            await _apiKeyRepository.GetByIdAndUserIdAsync(id, userId)
            ?? throw new NotFoundException($"No API Key with ID {id}");

        _apiKeyRepository.Delete(apiKey);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<ApiKeyResultDto> UpdateLabelAsync(Guid id, UpdateApiKeyDto dto, Guid userId)
    {
        var apiKey =
            await _apiKeyRepository.GetByIdAndUserIdAsync(id, userId)
            ?? throw new NotFoundException($"No API Key with ID {id}");

        apiKey.Label = dto.Label;
        _apiKeyRepository.Update(apiKey);
        await _unitOfWork.SaveChangesAsync();

        return new ApiKeyResultDto(
            Id: apiKey.Id,
            Label: apiKey.Label,
            MaskedKey: $"{apiKey.Prefix}••••••••••",
            CreatedDate: apiKey.CreatedDate,
            UpdatedDate: apiKey.UpdatedDate
        );
    }
}
