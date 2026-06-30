using TodoList.Api.Shared.Domain.Repositories;
using TodoList.Api.Features.Auth.Core.Entities;

namespace TodoList.Api.Features.Auth.Core.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
}
