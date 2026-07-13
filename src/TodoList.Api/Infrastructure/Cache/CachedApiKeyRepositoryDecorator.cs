using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using TodoList.Api.Application.Interfaces.Repositories;
using TodoList.Api.Domain.Entities;
using TodoList.Api.Infrastructure.Repositories;

namespace TodoList.Api.Infrastructure.Cache;

public class CachedApiKeyRepositoryDecorator : IApiKeyRepository
{
    private readonly IApiKeyRepository _inner;
    private readonly IDistributedCache _cache;
    private readonly TimeSpan _ttl;

    public CachedApiKeyRepositoryDecorator(
        IApiKeyRepository inner,
        IDistributedCache cache,
        IConfiguration configuration
    )
    {
        _inner = inner;
        _cache = cache;
        var ttlMinutes = configuration.GetValue("CacheSettings:ApiKeyTtlMinutes", 60);
        _ttl = TimeSpan.FromMinutes(ttlMinutes);
    }

    public async Task<ApiKey?> GetByIdAsync(Guid id)
    {
        var key = CacheKeys.ApiKey(id);
        var cached = await _cache.GetAsync<ApiKey>(key);
        if (cached != null)
        {
            return cached;
        }

        var apiKey = await _inner.GetByIdAsync(id);
        if (apiKey != null)
        {
            await _cache.SetAsync(key, apiKey, _ttl);
        }

        return apiKey;
    }

    public async Task<ApiKey?> GetByIdAndUserIdAsync(Guid id, Guid userId)
    {
        var key = CacheKeys.ApiKey(id);
        var cached = await _cache.GetAsync<ApiKey>(key);
        if (cached != null)
        {
            return cached.UserId == userId ? cached : null;
        }

        var apiKey = await _inner.GetByIdAndUserIdAsync(id, userId);
        if (apiKey != null)
        {
            await _cache.SetAsync(key, apiKey, _ttl);
        }

        return apiKey;
    }

    public async Task<IEnumerable<ApiKey>> GetAllByUserIdAsync(Guid userId)
    {
        var key = CacheKeys.ApiKeysByUser(userId);
        var cached = await _cache.GetAsync<List<ApiKey>>(key);
        if (cached != null)
        {
            return cached;
        }

        var apiKeys = (await _inner.GetAllByUserIdAsync(userId)).ToList();
        await _cache.SetAsync(key, apiKeys, _ttl);

        return apiKeys;
    }

    public async Task<int> CountActiveByUserIdAsync(Guid userId)
    {
        var key = CacheKeys.ApiKeysCountByUser(userId);
        var cached = await _cache.GetAsync<int?>(key);
        if (cached.HasValue)
        {
            return cached.Value;
        }

        var count = await _inner.CountActiveByUserIdAsync(userId);
        await _cache.SetAsync(key, count, _ttl);

        return count;
    }

    public async Task<IEnumerable<ApiKey>> GetActiveKeysByPrefixAsync(string prefix)
    {
        var key = CacheKeys.ApiKeysByPrefix(prefix);
        var cached = await _cache.GetAsync<List<ApiKey>>(key);
        if (cached != null)
        {
            return cached;
        }

        var apiKeys = (await _inner.GetActiveKeysByPrefixAsync(prefix)).ToList();
        await _cache.SetAsync(key, apiKeys, _ttl);

        return apiKeys;
    }

    public async Task<IEnumerable<ApiKey>> GetAllAsync()
    {
        // Skip caching for generic lists
        return await _inner.GetAllAsync();
    }

    public async Task AddAsync(ApiKey entity)
    {
        await _inner.AddAsync(entity);
        InvalidateCache(entity);
    }

    public void Update(ApiKey entity)
    {
        _inner.Update(entity);
        InvalidateCache(entity);
    }

    public void Delete(ApiKey entity)
    {
        _inner.Delete(entity);
        InvalidateCache(entity);
    }

    private void InvalidateCache(ApiKey entity)
    {
        _cache.Remove(CacheKeys.ApiKey(entity.Id));
        _cache.Remove(CacheKeys.ApiKeysByUser(entity.UserId));
        _cache.Remove(CacheKeys.ApiKeysCountByUser(entity.UserId));
        _cache.Remove(CacheKeys.ApiKeysByPrefix(entity.Prefix));
    }
}
