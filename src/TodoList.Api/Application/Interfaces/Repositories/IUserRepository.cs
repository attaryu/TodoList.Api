using TodoList.Api.Application.Interfaces.Repositories;
using TodoList.Api.Domain.Entities;

namespace TodoList.Api.Application.Interfaces.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
}
