using Microsoft.EntityFrameworkCore;
using TodoList.Api.Features.Auth.Core.Entities;
using TodoList.Api.Features.Auth.Core.Repositories;
using TodoList.Api.Shared.Infrastructure.Persistent;
using TodoList.Api.Shared.Infrastructure.Repositories;

namespace TodoList.Api.Features.Auth.Infrastructure.Persistents.Repositories;

public class UserRepositoryImpl(AppDbContext appDbContext)
    : BaseRepositoryImpl<User>(appDbContext),
        IUserRepository
{
    private readonly AppDbContext _context = appDbContext;

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
    }
}
