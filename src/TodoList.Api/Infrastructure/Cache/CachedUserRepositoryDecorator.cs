using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using TodoList.Api.Application.Interfaces.Repositories;
using TodoList.Api.Domain.Entities;
using TodoList.Api.Infrastructure.Repositories;

namespace TodoList.Api.Infrastructure.Cache;

public class CachedUserRepositoryDecorator : IUserRepository
{
    private readonly IUserRepository _inner;
    private readonly IDistributedCache _cache;
    private readonly TimeSpan _ttl;

    public CachedUserRepositoryDecorator(
        IUserRepository inner,
        IDistributedCache cache,
        IConfiguration configuration
    )
    {
        _inner = inner;
        _cache = cache;
        var ttlMinutes = configuration.GetValue("CacheSettings:UserTtlMinutes", 30);
        _ttl = TimeSpan.FromMinutes(ttlMinutes);
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        var key = CacheKeys.User(id);
        var cached = await _cache.GetAsync<User>(key);
        if (cached != null)
        {
            return cached;
        }

        var user = await _inner.GetByIdAsync(id);
        if (user != null)
        {
            await CacheUserAsync(user);
        }

        return user;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var key = CacheKeys.UserByEmail(email);
        var cached = await _cache.GetAsync<User>(key);
        if (cached != null)
        {
            return cached;
        }

        var user = await _inner.GetByEmailAsync(email);
        if (user != null)
        {
            await CacheUserAsync(user);
        }

        return user;
    }

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
    {
        var key = CacheKeys.UserByRefreshToken(refreshToken);
        var cached = await _cache.GetAsync<User>(key);
        if (cached != null)
        {
            return cached;
        }

        var user = await _inner.GetByRefreshTokenAsync(refreshToken);
        if (user != null)
        {
            await CacheUserAsync(user);
        }

        return user;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        // Skip caching for lists
        return await _inner.GetAllAsync();
    }

    public async Task AddAsync(User entity)
    {
        await _inner.AddAsync(entity);
        // Do not cache immediately since we'd rather do it on demand (GetByX calls)
    }

    public void Update(User entity)
    {
        // Prior to updating, we must retrieve the original cached values from database if we want to ensure we invalidate old emails/tokens.
        // But since we are updating the EF tracker and in-memory entity is updated, we can invalidate its keys.
        // To be safe, we invalidate the current user's keys.
        _inner.Update(entity);
        InvalidateUserCache(entity);
    }

    public void Delete(User entity)
    {
        _inner.Delete(entity);
        InvalidateUserCache(entity);
    }

    private async Task CacheUserAsync(User user)
    {
        var idKey = CacheKeys.User(user.Id);
        var emailKey = CacheKeys.UserByEmail(user.Email);

        await _cache.SetAsync(idKey, user, _ttl);
        await _cache.SetAsync(emailKey, user, _ttl);

        if (!string.IsNullOrEmpty(user.RefreshToken))
        {
            var tokenKey = CacheKeys.UserByRefreshToken(user.RefreshToken);
            await _cache.SetAsync(tokenKey, user, _ttl);
        }
    }

    private void InvalidateUserCache(User user)
    {
        _cache.Remove(CacheKeys.User(user.Id));
        _cache.Remove(CacheKeys.UserByEmail(user.Email));
        if (!string.IsNullOrEmpty(user.RefreshToken))
        {
            _cache.Remove(CacheKeys.UserByRefreshToken(user.RefreshToken));
        }
    }
}
