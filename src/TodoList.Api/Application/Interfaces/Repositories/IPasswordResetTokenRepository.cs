using TodoList.Api.Application.Interfaces.Repositories;
using TodoList.Api.Domain.Entities;

namespace TodoList.Api.Application.Interfaces.Repositories;

public interface IPasswordResetTokenRepository : IBaseRepository<PasswordResetToken>
{
    Task<PasswordResetToken?> GetByTokenAsync(string token);
    Task DeleteByUserIdAsync(Guid userId);
}
