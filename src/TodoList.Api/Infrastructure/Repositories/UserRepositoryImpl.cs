using Microsoft.EntityFrameworkCore;
using TodoList.Api.Application.Interfaces.Repositories;
using TodoList.Api.Domain.Entities;
using TodoList.Api.Infrastructure.DataContext;
using TodoList.Api.Infrastructure.Repositories;

namespace TodoList.Api.Infrastructure.Repositories;

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
