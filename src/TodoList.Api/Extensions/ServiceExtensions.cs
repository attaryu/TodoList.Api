using TodoList.Api.Application.Interfaces.Services;
using TodoList.Api.Application.Services;
using TodoList.Api.Common.Services;

namespace TodoList.Api.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenProvider, TokenProvider>();
        services.AddScoped<IHasherProvider, BCryptHasherProvider>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITodoService, TodoService>();
        services.AddHostedService<AppSeederService>();

        return services;
    }
}
