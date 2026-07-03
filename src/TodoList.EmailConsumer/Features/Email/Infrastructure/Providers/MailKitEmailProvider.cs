using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using TodoList.EmailConsumer.Features.Email.Core.Providers;

namespace TodoList.EmailConsumer.Features.Email.Infrastructure.Providers;

public class MailKitEmailProvider(
    IConfiguration configuration,
    ILogger<MailKitEmailProvider> logger
) : IEmailProvider
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<MailKitEmailProvider> _logger = logger;

    public async Task SendAsync(
        string to,
        string subject,
        string body,
        CancellationToken cancellationToken = default
    )
    {
        var smtpHost = _configuration["SMTP_HOST"] ?? "localhost";
        var smtpPortStr = _configuration["SMTP_PORT"] ?? "1025";
        var smtpUser = _configuration["SMTP_USER"] ?? "";
        var smtpPassword = _configuration["SMTP_PASSWORD"] ?? "";
        var fromEmail = _configuration["SMTP_FROM_EMAIL"] ?? "noreply@todolist.com";
        var fromName = _configuration["SMTP_FROM_NAME"] ?? "TodoList App";

        _ = int.TryParse(smtpPortStr, out var smtpPort);

        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress(fromName, fromEmail));
        emailMessage.To.Add(new MailboxAddress("", to));
        emailMessage.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = body };
        emailMessage.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(
                smtpHost,
                smtpPort,
                SecureSocketOptions.Auto,
                cancellationToken
            );

            if (!string.IsNullOrEmpty(smtpUser) && !string.IsNullOrEmpty(smtpPassword))
            {
                await client.AuthenticateAsync(smtpUser, smtpPassword, cancellationToken);
            }

            _logger.LogInformation("Sending email to {To}...", to);
            await client.SendAsync(emailMessage, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("Email sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {To}", to);
            throw;
        }
    }
}
