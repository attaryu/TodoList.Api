using TodoList.Api.Features.Auth.Core.Providers;
using TodoList.Api.Features.Auth.Core.Repositories;
using TodoList.Api.Features.Auth.Infrastructure.Persistents.Repositories;
using TodoList.Api.Features.Auth.Infrastructure.Providers;
using TodoList.Api.Shared.Domain.Providers;
using TodoList.Api.Shared.Domain.Repositories;
using TodoList.Api.Shared.Infrastructure.Providers;
using TodoList.Api.Shared.Infrastructure.Repositories;

namespace TodoList.Api.Shared.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreDependencies(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWorkImpl>();
        services.AddScoped<ITokenProvider, TokenProvider>();
        services.AddScoped<IUserRepository, UserRepositoryImpl>();
        services.AddScoped<IEmailVerificationRepository, EmailVerificationRepositoryImpl>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepositoryImpl>();
        services.AddScoped<IHasherProvider, BCryptHasherProvider>();

        return services;
    }
}
