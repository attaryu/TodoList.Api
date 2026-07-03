using TodoList.EmailConsumer.Features.Email.Core.Providers;

namespace TodoList.EmailConsumer.Features.Email.Core.UseCases;

public class SendEmailNotificationUseCase(IEmailProvider EmailProvider)
{
    private readonly IEmailProvider _EmailProvider = EmailProvider;

    public async Task ExecuteAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default
    )
    {
        await _EmailProvider.SendAsync(to, subject, body, cancellationToken);
    }
}
