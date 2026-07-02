using MassTransit;
using TodoList.Contracts.Commands;
using TodoList.EmailConsumer.Features.Email.Core.UseCases;

namespace TodoList.EmailConsumer.Features.Email.Infrastructure.Consumers;

public class SendEmailNotificationConsumer(SendEmailNotificationUseCase useCase)
    : IConsumer<SendEmailNotification>
{
    private readonly SendEmailNotificationUseCase _useCase = useCase;

    public async Task Consume(ConsumeContext<SendEmailNotification> context)
    {
        var message = context.Message;
        await _useCase.ExecuteAsync(
            message.To,
            message.Subject,
            message.Body,
            context.CancellationToken
        );
    }
}
