using TodoList.Api.Application.Interfaces.Repositories;
using TodoList.Api.Application.Interfaces.Services;
using TodoList.Api.Application.Services;
using TodoList.Api.Common.Services;
using TodoList.Api.Infrastructure.Repositories;

namespace TodoList.Api.Infrastructure;

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
        services.AddScoped<IAuthService, AuthService>();

        // Register Todo infrastructure & application services
        services.AddScoped<ITodoRepository, TodoRepositoryImpl>();
        services.AddScoped<ITodoService, TodoService>();
        services.AddHostedService<AppSeederService>();

        return services;
    }
}
