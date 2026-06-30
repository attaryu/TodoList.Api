namespace TodoList.Api.Features.Auth.Core.DTOs;

public class AuthResponseDto
{
    public UserResponseDto User { get; init; } = new();
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime AccessTokenExpiresAt { get; init; }
}
