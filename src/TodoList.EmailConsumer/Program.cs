using DotNetEnv;
using TodoList.EmailConsumer.Features.Email.Infrastructure;

string environment =
    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
    ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
    ?? "Development";

bool isNotInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true";

if (environment != "Production" || isNotInContainer)
{
    Env.TraversePath().Load($".env.{environment}");
}

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.AddEnvironmentVariables();
builder.Services.AddEmailDependencies();

var host = builder.Build();
host.Run();
