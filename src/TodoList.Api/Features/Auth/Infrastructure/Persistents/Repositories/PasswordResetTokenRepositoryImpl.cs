using Microsoft.EntityFrameworkCore;
using TodoList.Api.Features.Auth.Core.Entities;
using TodoList.Api.Features.Auth.Core.Repositories;
using TodoList.Api.Shared.Infrastructure.Persistent;
using TodoList.Api.Shared.Infrastructure.Repositories;

namespace TodoList.Api.Features.Auth.Infrastructure.Persistents.Repositories;

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
