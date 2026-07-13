using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using TodoList.Api.Application.Interfaces.Repositories;
using TodoList.Api.Domain.Entities;
using TodoList.Api.Infrastructure.Repositories;

namespace TodoList.Api.Infrastructure.Cache;

public class CachedTodoRepositoryDecorator : ITodoRepository
{
    private readonly ITodoRepository _inner;
    private readonly IDistributedCache _cache;
    private readonly TimeSpan _ttl;

    public CachedTodoRepositoryDecorator(
        ITodoRepository inner,
        IDistributedCache cache,
        IConfiguration configuration
    )
    {
        _inner = inner;
        _cache = cache;
        var ttlMinutes = configuration.GetValue("CacheSettings:TodoTtlMinutes", 10);
        _ttl = TimeSpan.FromMinutes(ttlMinutes);
    }

    public async Task<TodoItem?> GetByIdAsync(Guid id)
    {
        var key = CacheKeys.Todo(id);
        var cached = await _cache.GetAsync<TodoItem>(key);
        if (cached != null)
        {
            return cached;
        }

        var item = await _inner.GetByIdAsync(id);
        if (item != null)
        {
            await _cache.SetAsync(key, item, _ttl);
        }

        return item;
    }

    public async Task<TodoItem?> GetByIdAndUserIdAsync(Guid id, Guid userId)
    {
        var key = CacheKeys.Todo(id);
        var cached = await _cache.GetAsync<TodoItem>(key);
        if (cached != null)
        {
            return cached.UserId == userId ? cached : null;
        }

        var item = await _inner.GetByIdAndUserIdAsync(id, userId);
        if (item != null)
        {
            await _cache.SetAsync(key, item, _ttl);
        }

        return item;
    }

    public async Task<IEnumerable<TodoItem>> GetAllByUserIdAsync(Guid userId)
    {
        var key = CacheKeys.TodosByUser(userId);
        var cached = await _cache.GetAsync<List<TodoItem>>(key);
        if (cached != null)
        {
            return cached;
        }

        var items = (await _inner.GetAllByUserIdAsync(userId)).ToList();
        await _cache.SetAsync(key, items, _ttl);

        return items;
    }

    public async Task<(IEnumerable<TodoItem> Items, int TotalCount)> GetPagedByUserIdAsync(
        Guid userId,
        int page,
        int limit
    )
    {
        // Skip caching for paginated list query as requested
        return await _inner.GetPagedByUserIdAsync(userId, page, limit);
    }

    public async Task<IEnumerable<TodoItem>> GetAllAsync()
    {
        // Skip caching for general list queries
        return await _inner.GetAllAsync();
    }

    public async Task AddAsync(TodoItem entity)
    {
        await _inner.AddAsync(entity);

        // Invalidate user's list cache
        _cache.Remove(CacheKeys.TodosByUser(entity.UserId));
    }

    public void Update(TodoItem entity)
    {
        _inner.Update(entity);

        // Invalidate cache keys
        _cache.Remove(CacheKeys.Todo(entity.Id));
        _cache.Remove(CacheKeys.TodosByUser(entity.UserId));
    }

    public void Delete(TodoItem entity)
    {
        _inner.Delete(entity);

        // Invalidate cache keys
        _cache.Remove(CacheKeys.Todo(entity.Id));
        _cache.Remove(CacheKeys.TodosByUser(entity.UserId));
    }
}
