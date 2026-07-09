using TodoList.Api.Domain.Entities;

namespace TodoList.Api.Application.Interfaces.Services;

public interface ITokenProvider
{
    (string Token, DateTimeOffset ExpiresAt) GenerateAccessToken(User user);
    (string Token, DateTimeOffset ExpiresAt) GenerateRefreshToken();
}
