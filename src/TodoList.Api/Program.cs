using DotNetEnv;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using TodoList.Api.Configuration;
using TodoList.Api.Extensions;
using TodoList.Api.Infrastructure.DataContext;
using TodoList.Api.Presentation.Http.Middlewares;
using TodoList.Contracts.Commands;

string environment =
    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
    ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
    ?? "Development";

bool isNotInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true";

if (environment != "Production" || isNotInContainer)
{
    Env.TraversePath().Load($".env.{environment}");
}

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog Logging
LoggingConfiguration.ConfigureSerilog(builder);

// Configure Mapster Configuration mappings
MapsterConfiguration.Configure();

// Add Controllers and HTTP routing conventions
builder.Services.AddControllers();
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});

// Configure validation error response format
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errorMessage =
            context
                .ModelState.Values.SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .FirstOrDefault()
            ?? "Invalid request data.";

        var response = Sindika.AspNet.Response.ResponseHelper.Error<object, object>(
            null,
            "ERR-VAL-400",
            errorMessage
        );

        return new BadRequestObjectResult(response);
    };
});

// DbContext configuration using snake_case conventions
var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention()
);

// Add Validation dependencies
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();

// Add Extension Configurations
builder.Services.AddCorsPolicy();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddRepositories();
builder.Services.AddServices();

// Configure MassTransit with RabbitMQ
var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
var rabbitPort = Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672";
var rabbitUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest";
var rabbitPass = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest";

builder.Services.AddMassTransit(x =>
{
    EndpointConvention.Map<SendEmailNotification>(new Uri("queue:send-email-notification"));

    x.UsingRabbitMq(
        (context, cfg) =>
        {
            cfg.Host(
                $"rabbitmq://{rabbitHost}:{rabbitPort}",
                h =>
                {
                    h.Username(rabbitUser);
                    h.Password(rabbitPass);
                }
            );
        }
    );
});

var app = builder.Build();

// Setup Pipeline Middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment() || environment == "Development")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
