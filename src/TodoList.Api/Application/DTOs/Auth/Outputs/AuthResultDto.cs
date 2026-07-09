namespace TodoList.Api.Application.DTOs.Auth.Outputs;

public record AuthResultDto(
    UserResultDto User,
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessTokenExpiresAt
);
