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

        return services;
    }
}
