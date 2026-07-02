using System.Security.Cryptography;
using MassTransit;
using TodoList.Api.Features.Auth.Core.Entities;
using TodoList.Api.Features.Auth.Core.Repositories;
using TodoList.Api.Shared.Domain.Repositories;
using TodoList.Contracts.Commands;

namespace TodoList.Api.Features.Auth.Core.UseCases;

public class SendEmailVerificationUseCase(
    IUserRepository userRepository,
    IEmailVerificationRepository emailVerificationRepository,
    IUnitOfWork unitOfWork,
    ISendEndpointProvider sendEndpointProvider
)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IEmailVerificationRepository _emailVerificationRepository =
        emailVerificationRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ISendEndpointProvider _sendEndpointProvider = sendEndpointProvider;

    public async Task ExecuteAsync(int userId)
    {
        var user =
            await _userRepository.GetByIdAsync(userId)
            ?? throw new InvalidOperationException("User not found.");
        if (user.IsEmailVerified)
        {
            throw new InvalidOperationException("Email is already verified.");
        }

        // Delete any existing verification tokens
        await _emailVerificationRepository.DeleteByUserIdAsync(userId);

        // Generate secure verification token
        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        var token = Convert
            .ToBase64String(tokenBytes)
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "");

        var verification = new EmailVerification
        {
            UserId = userId,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(24),
            CreatedAt = DateTime.UtcNow,
        };

        await _emailVerificationRepository.AddAsync(verification);
        await _unitOfWork.SaveChangesAsync();

        // Construct HTML email body
        var emailBody = $"""
            <h1>Verify Your Email</h1>
            <p>Please use the following token to verify your email address:</p>
            <p><strong>{token}</strong></p>
            <p>This token will expire at {verification.ExpiresAt:yyyy-MM-dd HH:mm:ss} UTC.</p>
            """;

        // Send message to mapped endpoint
        await _sendEndpointProvider.Send(
            new SendEmailNotification(user.Email, "Verify Your Email Address", emailBody)
        );
    }
}
