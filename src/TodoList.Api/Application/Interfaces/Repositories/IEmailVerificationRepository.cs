using TodoList.Api.Application.Interfaces.Repositories;
using TodoList.Api.Domain.Entities;

namespace TodoList.Api.Application.Interfaces.Repositories;

public interface IEmailVerificationRepository : IBaseRepository<EmailVerification>
{
    Task<EmailVerification?> GetByUserIdAsync(int userId);
    Task<EmailVerification?> GetByTokenAsync(string token);
    Task DeleteByUserIdAsync(int userId);
}
