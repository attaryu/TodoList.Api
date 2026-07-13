using TodoList.Api.Application.Interfaces.Repositories;
using TodoList.Api.Infrastructure.Cache;
using TodoList.Api.Infrastructure.Repositories;

namespace TodoList.Api.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWorkImpl>();
        services.AddScoped<IEmailVerificationRepository, EmailVerificationRepositoryImpl>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepositoryImpl>();

        // Core repositories registered with their interfaces
        services.AddScoped<IUserRepository, UserRepositoryImpl>();
        services.AddScoped<ITodoRepository, TodoRepositoryImpl>();
        services.AddScoped<IApiKeyRepository, ApiKeyRepositoryImpl>();

        // Decorate core repositories with caching using Scrutor
        services.Decorate<IUserRepository, CachedUserRepositoryDecorator>();
        services.Decorate<ITodoRepository, CachedTodoRepositoryDecorator>();
        services.Decorate<IApiKeyRepository, CachedApiKeyRepositoryDecorator>();

        return services;
    }
}
