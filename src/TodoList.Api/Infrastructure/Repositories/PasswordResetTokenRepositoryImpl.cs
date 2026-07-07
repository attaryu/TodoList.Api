using Microsoft.EntityFrameworkCore;
using TodoList.Api.Application.Interfaces.Repositories;
using TodoList.Api.Domain.Entities;
using TodoList.Api.Infrastructure.DataContext;
using TodoList.Api.Infrastructure.Repositories;

namespace TodoList.Api.Infrastructure.Repositories;

public class PasswordResetTokenRepositoryImpl(AppDbContext appDbContext)
    : BaseRepositoryImpl<PasswordResetToken>(appDbContext),
        IPasswordResetTokenRepository
{
    private readonly AppDbContext _context = appDbContext;

    public async Task<PasswordResetToken?> GetByTokenAsync(string token)
    {
        return await _context
            .PasswordResetTokens.Include(prt => prt.User)
            .FirstOrDefaultAsync(prt => prt.Token == token);
    }

    public async Task DeleteByUserIdAsync(int userId)
    {
        var entities = await _context
            .PasswordResetTokens.Where(prt => prt.UserId == userId)
            .ToListAsync();

        if (entities.Count != 0)
        {
            _context.PasswordResetTokens.RemoveRange(entities);
        }
    }
}
