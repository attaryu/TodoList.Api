namespace TodoList.Api.Features.Auth.Core.DTOs.Outputs;

public record AuthResultDto(
    UserResultDto User,
    string AccessToken,
    string RefreshToken,
    DateTime AccessTokenExpiresAt
);
