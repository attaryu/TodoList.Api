using Microsoft.EntityFrameworkCore;
using TodoList.Api.Application.Interfaces.Repositories;
using TodoList.Api.Domain.Entities;
using TodoList.Api.Infrastructure.DataContext;
using TodoList.Api.Infrastructure.Repositories;

namespace TodoList.Api.Infrastructure.Repositories;

public class EmailVerificationRepositoryImpl(AppDbContext appDbContext)
    : BaseRepositoryImpl<EmailVerification>(appDbContext),
        IEmailVerificationRepository
{
    private readonly AppDbContext _context = appDbContext;

    public async Task<EmailVerification?> GetByUserIdAsync(int userId)
    {
        return await _context.EmailVerifications.FirstOrDefaultAsync(ev => ev.UserId == userId);
    }

    public async Task<EmailVerification?> GetByTokenAsync(string token)
    {
        return await _context
            .EmailVerifications.Include(ev => ev.User)
            .FirstOrDefaultAsync(ev => ev.Token == token);
    }

    public async Task DeleteByUserIdAsync(int userId)
    {
        var entities = await _context
            .EmailVerifications.Where(ev => ev.UserId == userId)
            .ToListAsync();

        if (entities.Count != 0)
        {
            _context.EmailVerifications.RemoveRange(entities);
        }
    }
}
