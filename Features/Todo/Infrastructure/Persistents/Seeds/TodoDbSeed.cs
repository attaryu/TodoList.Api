using TodoList.Api.Core.Infrastructure.Persistent;
using TodoList.Api.Features.Todo.Domain.Entities;

namespace TodoList.Api.Features.Todo.Infrastructure.Persistents.Seeds;

public static class TodoDbSeed
{
    public static async Task SeedTodoAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;

        var environment = services.GetRequiredService<IHostEnvironment>();
        if (!environment.IsDevelopment())
        {
            return;
        }

        var context = services.GetRequiredService<AppDbContext>();

        if (!context.TodoItems.Any())
        {
            var Todos = new List<TodoItem>
            {
                new()
                {
                    Id = 1,
                    Title = "Learn .NET 10",
                    Description = "Learn the new features and improvements in .NET 10.",
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new()
                {
                    Id = 2,
                    Title = "Build a Todo App",
                    Description = "Create a simple todo application using .NET 10.",
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            await context.TodoItems.AddRangeAsync(Todos);
            await context.SaveChangesAsync();
        }
    }
}
