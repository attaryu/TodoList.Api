namespace TodoList.Api.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .WithOrigins(
                        Environment.GetEnvironmentVariable("CORS_ORIGINS")?.Split(',')
                            ?? ["http://localhost:3000"]
                    );
            });
        });

        return services;
    }
}
