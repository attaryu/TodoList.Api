using Microsoft.EntityFrameworkCore;
using TodoList.Api.Features.Auth.Core.Entities;
using TodoList.Api.Features.Auth.Core.Repositories;
using TodoList.Api.Shared.Infrastructure.Persistent;
using TodoList.Api.Shared.Infrastructure.Repositories;

namespace TodoList.Api.Features.Auth.Infrastructure.Persistents.Repositories;

public class EmailVerificationRepositoryImpl(AppDbContext appDbContext)
    : BaseRepositoryImpl<EmailVerification>(appDbContext),
        IEmailVerificationRepository
{
    private readonly AppDbContext _context = appDbContext;

    public async Task<EmailVerification?> GetByUserIdAsync(int userId)
    {
        return await _context.EmailVerifications.FirstOrDefaultAsync(ev => ev.UserId == userId);
    }

    public async Task DeleteByUserIdAsync(int userId)
    {
        var entities = await _context
            .EmailVerifications.Where(ev => ev.UserId == userId)
            .ToListAsync();

        if (entities.Any())
        {
            _context.EmailVerifications.RemoveRange(entities);
        }
    }
}
