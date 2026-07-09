namespace TodoList.Api.Application.DTOs.Auth.Outputs;

public record AuthResponseDto(
    UserResultDto User,
    string AccessToken,
    DateTimeOffset AccessTokenExpiresAt
);
