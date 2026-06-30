using TodoList.Api.Core.Domain.Repositories;
using TodoList.Api.Core.Infrastructure.Repositories;

namespace TodoList.Api.Core.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreDependencies(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWorkImpl>();

        return services;
    }
}
