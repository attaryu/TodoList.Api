using Serilog;

namespace TodoList.Api.Configuration;

public static class LoggingConfiguration
{
    public static void ConfigureSerilog(WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, configuration) => configuration
            .ReadFrom.Configuration(context.Configuration)
            .WriteTo.Console()
        );
    }
}
