using TodoList.Api.Features.Auth.Core.Repositories;
using TodoList.Api.Features.Auth.Core.UseCases;
using TodoList.Api.Features.Auth.Infrastructure.Persistents.Repositories;
using TodoList.Api.Features.Auth.Core.Providers;
using TodoList.Api.Features.Auth.Infrastructure.Providers;

namespace TodoList.Api.Features.Auth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthDependencies(this IServiceCollection services)
    {
        services.AddAuthInfrastructure();
        services.AddAuthUseCases();

        return services;
    }

    private static IServiceCollection AddAuthUseCases(this IServiceCollection services)
    {
        services.AddScoped<RegisterUserUseCase>();
        services.AddScoped<LoginUserUseCase>();
        services.AddScoped<RefreshTokenUseCase>();
        services.AddScoped<LogoutUserUseCase>();

        return services;
    }

    private static IServiceCollection AddAuthInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepositoryImpl>();
        services.AddScoped<IHasherProvider, BCryptHasherProvider>();

        return services;
    }
}
