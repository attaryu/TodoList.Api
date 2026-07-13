using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sindika.AspNet.Response;
using TodoList.Api.Application.DTOs.ApiKey.Inputs;
using TodoList.Api.Application.DTOs.ApiKey.Outputs;
using TodoList.Api.Application.Interfaces.Services;
using TodoList.Api.Presentation.Http.Controllers.Base;

namespace TodoList.Api.Presentation.Http.Controllers;

[Authorize]
[Route("api/[controller]")]
public class ApiKeyController(IApiKeyService apiKeyService) : BaseApiController
{
    private readonly IApiKeyService _apiKeyService = apiKeyService;

    [HttpGet]
    [ProducesResponseType(
        typeof(BaseResponse<IEnumerable<ApiKeyResultDto>, object>),
        StatusCodes.Status200OK
    )]
    public async Task<IActionResult> GetApiKeys()
    {
        var userId = GetCurrentUserId();
        var apiKeys = await _apiKeyService.GetAllByUserIdAsync(userId);

        return Ok(
            ResponseHelper.Success<IEnumerable<ApiKeyResultDto>>(
                apiKeys,
                "API keys retrieved successfully."
            )
        );
    }

    [HttpPost]
    [ProducesResponseType(
        typeof(BaseResponse<ApiKeyCreatedResultDto, object>),
        StatusCodes.Status200OK
    )]
    public async Task<IActionResult> CreateApiKey([FromBody] CreateApiKeyDto request)
    {
        var userId = GetCurrentUserId();
        var createdKey = await _apiKeyService.CreateAsync(request, userId);

        return Ok(
            ResponseHelper.Success<ApiKeyCreatedResultDto>(
                createdKey,
                "API key created successfully."
            )
        );
    }

    [HttpDelete("{id}/revoke")]
    [ProducesResponseType(typeof(BaseResponse<object, object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> RevokeApiKey(Guid id)
    {
        var userId = GetCurrentUserId();
        await _apiKeyService.RevokeAsync(id, userId);

        return Ok(ResponseHelper.Success<object>(null, "API key revoked successfully."));
    }

    [HttpPut("{id}")]
    [ProducesResponseType(typeof(BaseResponse<ApiKeyResultDto, object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateApiKey(Guid id, [FromBody] UpdateApiKeyDto request)
    {
        var userId = GetCurrentUserId();
        var updatedKey = await _apiKeyService.UpdateLabelAsync(id, request, userId);

        return Ok(
            ResponseHelper.Success<ApiKeyResultDto>(
                updatedKey,
                "API key label updated successfully."
            )
        );
    }
}
