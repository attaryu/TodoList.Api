using TodoList.Api.Application.Interfaces.Repositories;
using TodoList.Api.Infrastructure.Repositories;

namespace TodoList.Api.Extensions;

public static class RepositoryExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWorkImpl>();
        services.AddScoped<IUserRepository, UserRepositoryImpl>();
        services.AddScoped<IEmailVerificationRepository, EmailVerificationRepositoryImpl>();
        services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepositoryImpl>();
        services.AddScoped<ITodoRepository, TodoRepositoryImpl>();

        return services;
    }
}
