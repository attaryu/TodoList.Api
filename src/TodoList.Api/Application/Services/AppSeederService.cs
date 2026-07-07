using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TodoList.Api.Domain.Entities;
using TodoList.Api.Features.Auth.Core.Providers;
using TodoList.Api.Shared.Infrastructure.Persistent;

namespace TodoList.Api.Application.Services;

public class AppSeederService(IServiceProvider serviceProvider) : IHostedService
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        var environment = services.GetRequiredService<IHostEnvironment>();
        if (!environment.IsDevelopment())
        {
            return;
        }

        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync(cancellationToken);

        var firstUser = await context.Users.FirstOrDefaultAsync(cancellationToken);

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

            await context.Users.AddAsync(firstUser, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }

        if (!await context.TodoItems.AnyAsync(cancellationToken))
        {
            var todos = new List<TodoItem>
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

            await context.TodoItems.AddRangeAsync(todos, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
