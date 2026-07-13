using MessagePack;
using MessagePack.Resolvers;
using Microsoft.Extensions.Caching.Distributed;

namespace TodoList.Api.Infrastructure.Cache;

public static class CacheExtensions
{
    private static readonly MessagePackSerializerOptions Options =
        MessagePackSerializerOptions.Standard.WithResolver(ContractlessStandardResolver.Instance);

    public static async Task<T?> GetAsync<T>(this IDistributedCache cache, string key)
    {
        var bytes = await cache.GetAsync(key);
        if (bytes == null || bytes.Length == 0)
        {
            return default;
        }

        try
        {
            return MessagePackSerializer.Deserialize<T>(bytes, Options);
        }
        catch
        {
            // If deserialization fails (e.g., due to schema updates), treat it as a cache miss
            return default;
        }
    }

    public static async Task SetAsync<T>(
        this IDistributedCache cache,
        string key,
        T value,
        TimeSpan? ttl = null
    )
    {
        if (value == null)
            return;

        var bytes = MessagePackSerializer.Serialize(value, Options);
        var options = new DistributedCacheEntryOptions();
        if (ttl.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = ttl.Value;
        }

        await cache.SetAsync(key, bytes, options);
    }
}
