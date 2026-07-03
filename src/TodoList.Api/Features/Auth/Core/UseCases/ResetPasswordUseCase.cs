using TodoList.Api.Features.Auth.Core.DTOs.Inputs;
using TodoList.Api.Features.Auth.Core.Providers;
using TodoList.Api.Features.Auth.Core.Repositories;
using TodoList.Api.Shared.Domain.Repositories;

namespace TodoList.Api.Features.Auth.Core.UseCases;

public class ResetPasswordUseCase(
    IPasswordResetTokenRepository passwordResetTokenRepository,
    IUserRepository userRepository,
    IHasherProvider hasherProvider,
    IUnitOfWork unitOfWork
)
{
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository =
        passwordResetTokenRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IHasherProvider _hasherProvider = hasherProvider;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<string> ExecuteAsync(ResetPasswordDto resetPasswordDto)
    {
        var passwordResetToken = await _passwordResetTokenRepository.GetByTokenAsync(
            resetPasswordDto.Token
        );

        if (passwordResetToken == null)
        {
            return GetErrorHtml("Reset password token is invalid or not found.");
        }

        if (passwordResetToken.ExpiresAt < DateTime.UtcNow)
        {
            await _passwordResetTokenRepository.DeleteByUserIdAsync(passwordResetToken.UserId);
            await _unitOfWork.SaveChangesAsync();
            return GetErrorHtml("Reset password token has expired. Please request a new link.");
        }

        var user = passwordResetToken.User;
        if (user == null)
        {
            return GetErrorHtml("User associated with this token is not found.");
        }

        user.PasswordHash = _hasherProvider.HashText(resetPasswordDto.Password);
        user.RefreshToken = null;
        user.RefreshTokenExpiresAt = null;
        user.UpdatedAt = DateTime.UtcNow;

        _userRepository.Update(user);
        await _passwordResetTokenRepository.DeleteByUserIdAsync(user.Id);
        await _unitOfWork.SaveChangesAsync();

        return GetSuccessHtml(user.Fullname);
    }

    private static string GetSuccessHtml(string fullname)
    {
        return $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Password Reset Successful</title>
                <style>
                    body {
                        font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif;
                        background-color: #f4f7f6;
                        color: #333333;
                        display: flex;
                        align-items: center;
                        justify-content: center;
                        height: 100vh;
                        margin: 0;
                    }
                    .container {
                        background-color: #ffffff;
                        padding: 40px;
                        border-radius: 8px;
                        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
                        text-align: center;
                        max-width: 450px;
                        width: 100%;
                    }
                    .icon {
                        font-size: 48px;
                        color: #2ecc71;
                        margin-bottom: 20px;
                    }
                    h1 {
                        font-size: 24px;
                        margin-bottom: 10px;
                        color: #2c3e50;
                    }
                    p {
                        font-size: 16px;
                        line-height: 1.5;
                        color: #7f8c8d;
                        margin-bottom: 30px;
                    }
                </style>
            </head>
            <body>
                <div class="container">
                    <div class="icon">✓</div>
                    <h1>Password Reset Successful, {{fullname}}!</h1>
                    <p>Your password has been successfully updated. You can now use your new password to log in to your account.</p>
                </div>
            </body>
            </html>
            """;
    }

    private static string GetErrorHtml(string errorMessage)
    {
        return $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Reset Password Failed</title>
                <style>
                    body {
                        font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, Helvetica, Arial, sans-serif;
                        background-color: #f4f7f6;
                        color: #333333;
                        display: flex;
                        align-items: center;
                        justify-content: center;
                        height: 100vh;
                        margin: 0;
                    }
                    .container {
                        background-color: #ffffff;
                        padding: 40px;
                        border-radius: 8px;
                        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05);
                        text-align: center;
                        max-width: 450px;
                        width: 100%;
                    }
                    .icon {
                        font-size: 48px;
                        color: #e74c3c;
                        margin-bottom: 20px;
                    }
                    h1 {
                        font-size: 24px;
                        margin-bottom: 10px;
                        color: #2c3e50;
                    }
                    p {
                        font-size: 16px;
                        line-height: 1.5;
                        color: #7f8c8d;
                        margin-bottom: 30px;
                    }
                </style>
            </head>
            <body>
                <div class="container">
                    <div class="icon">✗</div>
                    <h1>Reset Password Failed</h1>
                    <p>{{errorMessage}}</p>
                    <button onclick="window.history.back()" style="padding: 10px 20px; font-size: 16px; background-color: black; color: white; border: none; border-radius: 5px; cursor: pointer;">Go Back</button>
                </div>
            </body>
            </html>
            """;
    }
}
