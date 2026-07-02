using System.Text;
using DotNetEnv;
using FluentValidation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;
using TodoList.Api.Features.Auth.Infrastructure;
using TodoList.Api.Features.Todo.Infrastructure;
using TodoList.Api.Features.Todo.Infrastructure.Persistents.Seeds;
using TodoList.Api.Shared.Contracts;
using TodoList.Api.Shared.Infrastructure;
using TodoList.Api.Shared.Infrastructure.Persistent;
using TodoList.Api.Shared.Presentation.Helpers;

string environment =
    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
    ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT")
    ?? "Development";

if (environment != "Production")
{
    Env.TraversePath().Load($".env.{environment}");
}

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TodoList API", Version = "v1" });
    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Description =
                "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
        }
    );

    c.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        { new OpenApiSecuritySchemeReference("Bearer", doc, null), new List<string>() },
    });
});
builder.Configuration.AddEnvironmentVariables();

var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention()
);

builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});

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

        var response = ApiResponseHelper.Error(400, "Validation Failed", errorMessage);

        return new BadRequestObjectResult(response);
    };
});

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();

// Configure JWT Authentication
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
if (string.IsNullOrEmpty(jwtSecret))
{
    throw new InvalidOperationException("JWT_SECRET environment variable is not configured.");
}

builder
    .Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER"),
            ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero,
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var response = TodoList.Api.Shared.Presentation.Helpers.ApiResponseHelper.Error(
                    StatusCodes.Status401Unauthorized,
                    "Unauthorized",
                    "Authentication token is missing, invalid, or expired."
                );

                var json = System.Text.Json.JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                var response = TodoList.Api.Shared.Presentation.Helpers.ApiResponseHelper.Error(
                    StatusCodes.Status403Forbidden,
                    "Forbidden",
                    "You do not have permission to access this resource."
                );

                var json = System.Text.Json.JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(json);
            },
        };
    });

builder.Services.AddCoreDependencies();
builder.Services.AddTodoDependencies();
builder.Services.AddAuthDependencies();

// Configure MassTransit with RabbitMQ
var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
var rabbitPort = Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672";
var rabbitUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest";
var rabbitPass = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest";

EndpointConvention.Map<SendEmailNotification>(
    new Uri("exchange:email-notification?type=direct&routingKey=send-email")
);

builder.Services.AddMassTransit(x =>
{
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

            // Configure RabbitMQ topologies for direct exchange and queue binding
            cfg.ReceiveEndpoint(
                "email-notification",
                re =>
                {
                    re.ConfigureConsumeTopology = false;

                    re.Bind(
                        "email-notification",
                        s =>
                        {
                            s.RoutingKey = "send-email";
                            s.ExchangeType = RabbitMQ.Client.ExchangeType.Direct;
                        }
                    );
                }
            );
        }
    );
});

var app = builder.Build();

if (app.Environment.IsDevelopment() || environment == "Development")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// data seeding
await app.SeedTodoAsync();

app.Run();
