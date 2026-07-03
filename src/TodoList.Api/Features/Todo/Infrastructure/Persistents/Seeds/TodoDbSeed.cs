using Microsoft.EntityFrameworkCore;
using TodoList.Api.Features.Auth.Core.Entities;
using TodoList.Api.Features.Auth.Core.Providers;
using TodoList.Api.Features.Todo.Core.Entities;
using TodoList.Api.Shared.Infrastructure.Persistent;

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
        await context.Database.MigrateAsync();

        var firstUser = await context.Users.FirstOrDefaultAsync();

        if (firstUser == null)
        {
            var hasher = services.GetRequiredService<IHasherProvider>();
            firstUser = new User
            {
                Fullname = "Default User",
                Email = "user@example.com",
                PasswordHash = hasher.HashText("password"),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            await context.Users.AddAsync(firstUser);
            await context.SaveChangesAsync();
        }

        if (!context.TodoItems.Any())
        {
            var Todos = new List<TodoItem>
            {
                new()
                {
                    UserId = firstUser.Id,
                    Title = "Learn .NET 10",
                    Description = "Learn the new features and improvements in .NET 10.",
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                },
                new()
                {
                    UserId = firstUser.Id,
                    Title = "Build a Todo App",
                    Description = "Create a simple todo application using .NET 10.",
                    IsCompleted = true,
                    CompletedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                },
            };

            await context.TodoItems.AddRangeAsync(Todos);
            await context.SaveChangesAsync();
        }
    }
}
