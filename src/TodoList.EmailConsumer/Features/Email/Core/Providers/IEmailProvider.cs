namespace TodoList.EmailConsumer.Features.Email.Core.Providers;

public interface IEmailProvider
{
    Task SendAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default
    );
}
