namespace TodoList.Api.Infrastructure.Cache;

public static class CacheKeys
{
    // Todo keys
    public static string Todo(Guid id) => $"todo:{id}";

    public static string TodosByUser(Guid userId) => $"todos:user:{userId}";

    // User keys
    public static string User(Guid id) => $"user:{id}";

    public static string UserByEmail(string email) => $"user:email:{email.ToLowerInvariant()}";

    public static string UserByRefreshToken(string token) => $"user:token:{token}";

    // ApiKey keys
    public static string ApiKey(Guid id) => $"apikey:{id}";

    public static string ApiKeysByUser(Guid userId) => $"apikeys:user:{userId}";

    public static string ApiKeysCountByUser(Guid userId) => $"apikeys:count:user:{userId}";

    public static string ApiKeysByPrefix(string prefix) => $"apikeys:prefix:{prefix}";
}
