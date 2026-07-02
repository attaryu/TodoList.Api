using DotNetEnv;
using TodoList.EmailConsumer.Features.Email.Infrastructure;

string environment =
    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
    ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
    ?? "Development";

if (environment != "Production")
{
    Env.TraversePath().Load($".env.{environment}");
}

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddEmailDependencies();

var host = builder.Build();
host.Run();
