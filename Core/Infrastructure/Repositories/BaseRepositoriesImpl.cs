using Microsoft.EntityFrameworkCore;
using TodoList.Api.Core.Domain.Repositories;
using TodoList.Api.Core.Infrastructure.Persistent;

namespace TodoList.Api.Core.Infrastructure.Repositories;

public class BaseRepositoryImpl<T>(AppDbContext appDbContext) : IBaseRepository<T> where T : class
{
    protected readonly DbSet<T> _dbSet = appDbContext.Set<T>();

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }
}
