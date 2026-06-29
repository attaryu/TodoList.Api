using Microsoft.EntityFrameworkCore;
using DotNetEnv;
using TodoList.Api.Core.Infrastructure.Persistent;
using TodoList.Api.Features.Todo.Infrastructure;
using TodoList.Api.Core.Infrastructure;
using TodoList.Api.Features.Todo.Infrastructure.Persistents.Seeds;

string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
    ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
    ?? "Development";

string envFile = $".env.{environment}";

if (File.Exists(envFile) && environment != "Production")
{
    Env.Load(envFile);
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration.AddEnvironmentVariables();

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

builder.Services.AddDbContext<AppDbContext>(options =>
    options
        .UseNpgsql(connectionString)
        .UseSnakeCaseNamingConvention());

// dependency registration
builder.Services.AddCoreDependencies();
builder.Services.AddTodoDependencies();

var app = builder.Build();

if (app.Environment.IsDevelopment() || environment == "Development")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// data seeding
await app.SeedTodoItemAsync();

app.Run();
