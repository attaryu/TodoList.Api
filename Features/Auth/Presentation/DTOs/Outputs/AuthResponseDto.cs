using TodoList.Api.Features.Auth.Core.DTOs.Outputs;

namespace TodoList.Api.Features.Auth.Presentation.DTOs.Outputs;

public record AuthResponseDto(
    UserResultDto User,
    string AccessToken,
    DateTime AccessTokenExpiresAt
);
