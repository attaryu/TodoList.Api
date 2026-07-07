using TodoList.Api.Application.Interfaces.Services;
using TodoList.Api.Application.Services;

namespace TodoList.Api.Features.Auth.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddAuthDependencies(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
