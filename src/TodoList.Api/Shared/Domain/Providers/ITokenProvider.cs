using TodoList.Api.Features.Auth.Core.Entities;

namespace TodoList.Api.Shared.Domain.Providers;

public interface ITokenProvider
{
    (string Token, DateTime ExpiresAt) GenerateAccessToken(User user);
    (string Token, DateTime ExpiresAt) GenerateRefreshToken();
}
