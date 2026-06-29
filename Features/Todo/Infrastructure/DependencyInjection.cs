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
        services.AddScoped<GetTodoItemUseCase>();
        services.AddScoped<GetTodoItemsUseCase>();
        services.AddScoped<CreateTodoItemUseCase>();
        services.AddScoped<UpdateTodoItemUseCase>();
        services.AddScoped<DeleteTodoItemUseCase>();
        services.AddScoped<ToggleTodoItemUseCase>();

        return services;
    }

    private static IServiceCollection AddTodoInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ITodoRepository, TodoRepositoryImpl>();

        return services;
    }
}
