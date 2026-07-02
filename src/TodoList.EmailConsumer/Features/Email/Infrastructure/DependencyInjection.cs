using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using TodoList.EmailConsumer.Features.Email.Core.Providers;
using TodoList.EmailConsumer.Features.Email.Core.UseCases;
using TodoList.EmailConsumer.Features.Email.Infrastructure.Consumers;
using TodoList.EmailConsumer.Features.Email.Infrastructure.Providers;

namespace TodoList.EmailConsumer.Features.Email.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddEmailDependencies(this IServiceCollection services)
    {
        services.AddTransient<IEmailProvider, MailKitEmailProvider>();
        services.AddTransient<SendEmailNotificationUseCase>();

        var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost";
        var rabbitPort = Environment.GetEnvironmentVariable("RABBITMQ_PORT") ?? "5672";
        var rabbitUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "guest";
        var rabbitPass = Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest";

        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();

            x.AddConsumer<SendEmailNotificationConsumer>();

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

                    cfg.ConfigureEndpoints(context);
                }
            );
        });

        return services;
    }
}
