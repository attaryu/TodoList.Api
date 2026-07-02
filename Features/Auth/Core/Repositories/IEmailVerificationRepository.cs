using TodoList.Api.Features.Auth.Core.Entities;
using TodoList.Api.Shared.Domain.Repositories;

namespace TodoList.Api.Features.Auth.Core.Repositories;

public interface IEmailVerificationRepository : IBaseRepository<EmailVerification>
{
    Task<EmailVerification?> GetByUserIdAsync(int userId);
    Task DeleteByUserIdAsync(int userId);
}
