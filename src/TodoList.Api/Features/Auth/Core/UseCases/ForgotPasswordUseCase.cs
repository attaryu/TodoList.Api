using System.Security.Cryptography;
using MassTransit;
using TodoList.Api.Features.Auth.Core.DTOs.Inputs;
using TodoList.Api.Features.Auth.Core.Entities;
using TodoList.Api.Features.Auth.Core.Repositories;
using TodoList.Api.Shared.Domain.Repositories;
using TodoList.Contracts.Commands;

namespace TodoList.Api.Features.Auth.Core.UseCases;

public class ForgotPasswordUseCase(
    IUserRepository userRepository,
    IPasswordResetTokenRepository passwordResetTokenRepository,
    IUnitOfWork unitOfWork,
    ISendEndpointProvider sendEndpointProvider,
    IConfiguration configuration
)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository =
        passwordResetTokenRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ISendEndpointProvider _sendEndpointProvider = sendEndpointProvider;
    private readonly IConfiguration _configuration = configuration;

    public async Task ExecuteAsync(ForgotPasswordDto forgotPasswordDto)
    {
        var user = await _userRepository.GetByEmailAsync(forgotPasswordDto.Email);

        // If email is not registered, ignore silently
        if (user == null)
        {
            return;
        }

        await _passwordResetTokenRepository.DeleteByUserIdAsync(user.Id);

        var tokenBytes = RandomNumberGenerator.GetBytes(32);
        var token = Convert
            .ToBase64String(tokenBytes)
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "");

        var passwordResetToken = new PasswordResetToken
        {
            UserId = user.Id,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            CreatedAt = DateTime.UtcNow,
        };

        await _passwordResetTokenRepository.AddAsync(passwordResetToken);
        await _unitOfWork.SaveChangesAsync();

        var domain = _configuration["DOMAIN"];

        var emailBody = $"""
            <h1>Reset Your Password</h1>
            <p>Please use click the button below to reset your password:</p>
            <button style="padding: 10px 20px; font-size: 16px; background-color: black; color: white; border: none; border-radius: 5px; cursor: pointer;">
                <a href="{domain}/reset-password?token={passwordResetToken.Token}" style="color: white; text-decoration: none;" target="_blank">
                    Reset Password
                </a>
            </button>
            <p>This link will expire in 15 minutes from {DateTime.UtcNow:dd MMM yyyy HH:mm:ss} UTC.</p>
            """;

        await _sendEndpointProvider.Send(
            new SendEmailNotification(user.Email, "Reset Your Password", emailBody)
        );
    }
}
