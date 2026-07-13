using Microsoft.EntityFrameworkCore;
using TodoList.Api.Application.Interfaces.Repositories;
using TodoList.Api.Domain.Entities;
using TodoList.Api.Infrastructure.DataContext;

namespace TodoList.Api.Infrastructure.Repositories;

public class ApiKeyRepositoryImpl(AppDbContext appDbContext)
    : BaseRepositoryImpl<ApiKey>(appDbContext),
        IApiKeyRepository
{
    public async Task<IEnumerable<ApiKey>> GetAllByUserIdAsync(Guid userId)
    {
        return await _dbSet.Where(k => k.UserId == userId).ToListAsync();
    }

    public async Task<ApiKey?> GetByIdAndUserIdAsync(Guid id, Guid userId)
    {
        return await _dbSet.FirstOrDefaultAsync(k => k.Id == id && k.UserId == userId);
    }

    public async Task<int> CountActiveByUserIdAsync(Guid userId)
    {
        return await _dbSet.CountAsync(k => k.UserId == userId);
    }
}
