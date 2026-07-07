using TodoList.Api.Features.Auth.Core.Entities;
using TodoList.Api.Shared.Domain.Repositories;

namespace TodoList.Api.Features.Auth.Core.Repositories;

public interface IPasswordResetTokenRepository : IBaseRepository<PasswordResetToken>
{
    Task<PasswordResetToken?> GetByTokenAsync(string token);
    Task DeleteByUserIdAsync(int userId);
}
