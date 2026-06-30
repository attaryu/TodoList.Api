using TodoList.Api.Features.Todo.Domain.Repositories;
using TodoList.Api.Features.Todo.Domain.UseCases;
using TodoList.Api.Features.Todo.Infrastructure.Persistents.Repositories;

namespace TodoList.Api.Features.Todo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddTodoDependencies(this IServiceCollection services)
    {
        services.AddTodoInfrastructure();
        services.AddTodoUseCases();

        return services;
    }

    private static IServiceCollection AddTodoUseCases(this IServiceCollection services)
    {
        services.AddScoped<GetTodoUseCase>();
        services.AddScoped<GetTodosUseCase>();
        services.AddScoped<CreateTodoUseCase>();
        services.AddScoped<UpdateTodoUseCase>();
        services.AddScoped<DeleteTodoUseCase>();
        services.AddScoped<ToggleTodoUseCase>();

        return services;
    }

    private static IServiceCollection AddTodoInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITodoRepository, TodoRepositoryImpl>();

        return services;
    }
}
