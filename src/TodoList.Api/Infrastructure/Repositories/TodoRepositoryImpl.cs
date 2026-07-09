using Microsoft.EntityFrameworkCore;
using TodoList.Api.Application.Interfaces.Repositories;
using TodoList.Api.Domain.Entities;
using TodoList.Api.Infrastructure.DataContext;
using TodoList.Api.Infrastructure.Repositories;

namespace TodoList.Api.Infrastructure.Repositories;

public class TodoRepositoryImpl(AppDbContext appDbContext)
    : BaseRepositoryImpl<TodoItem>(appDbContext),
        ITodoRepository
{
    public async Task<IEnumerable<TodoItem>> GetAllByUserIdAsync(Guid userId)
    {
        return await _dbSet.Where(t => t.UserId == userId).ToListAsync();
    }

    public async Task<TodoItem?> GetByIdAndUserIdAsync(Guid id, Guid userId)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    }

    public async Task<(IEnumerable<TodoItem> Items, int TotalCount)> GetPagedByUserIdAsync(
        Guid userId,
        int page,
        int limit
    )
    {
        var query = _dbSet.Where(t => t.UserId == userId);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(t => t.UpdatedDate ?? t.CreatedDate)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (items, totalCount);
    }
}
