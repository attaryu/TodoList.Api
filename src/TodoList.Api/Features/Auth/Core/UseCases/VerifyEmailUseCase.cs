using TodoList.Api.Features.Auth.Core.Repositories;
using TodoList.Api.Shared.Domain.Repositories;

namespace TodoList.Api.Features.Auth.Core.UseCases;

public class VerifyEmailUseCase(
    IEmailVerificationRepository emailVerificationRepository,
    IUserRepository userRepository,
    IUnitOfWork unitOfWork
)
{
    private readonly IEmailVerificationRepository _emailVerificationRepository =
        emailVerificationRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<string> ExecuteAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return GetErrorHtml("Verification token cannot be empty or invalid.");
        }

        var verification = await _emailVerificationRepository.GetByTokenAsync(token);

        if (verification == null)
        {
            return GetErrorHtml(
                "Verification token is invalid or not found. Please request a new email verification."
            );
        }

        if (verification.ExpiresAt < DateTime.UtcNow)
        {
            await _emailVerificationRepository.DeleteByUserIdAsync(verification.UserId);
            await _unitOfWork.SaveChangesAsync();

            return GetErrorHtml(
                "Verification token has expired. Please request a new email verification."
            );
        }

        var user = verification.User;
        if (user == null)
        {
            return GetErrorHtml("User associated with this token is not found.");
        }

        if (!user.IsEmailVerified)
        {
            user.IsEmailVerified = true;
            user.UpdatedAt = DateTime.UtcNow;
            _userRepository.Update(user);
        }

        await _emailVerificationRepository.DeleteByUserIdAsync(user.Id);
        await _unitOfWork.SaveChangesAsync();

        return GetSuccessHtml(user.Fullname, user.Email);
    }

    private static string GetSuccessHtml(string fullname, string email)
    {
        return $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Email Verification Successful</title>
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
                    .user-info {
                        background-color: #f8f9fa;
                        padding: 15px;
                        border-radius: 6px;
                        margin-bottom: 30px;
                        font-size: 14px;
                        color: #555555;
                        text-align: left;
                    }
                    .user-info div {
                        margin-bottom: 5px;
                    }
                    .btn {
                        display: inline-block;
                        padding: 12px 24px;
                        font-size: 16px;
                        font-weight: 600;
                        background-color: #000000;
                        color: #ffffff;
                        text-decoration: none;
                        border-radius: 6px;
                        transition: background-color 0.2s ease;
                    }
                    .btn:hover {
                        background-color: #333333;
                    }
                </style>
            </head>
            <body>
                <div class="container">
                    <div class="icon">✓</div>
                    <h1>Verification Successful, {{fullname}}!</h1>
                    <p>Congratulations, your email {{email}} has been successfully verified. Your account is now fully active.</p>
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
                <title>Email Verification Failed</title>
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
                    .btn {
                        display: inline-block;
                        padding: 12px 24px;
                        font-size: 16px;
                        font-weight: 600;
                        background-color: #000000;
                        color: #ffffff;
                        text-decoration: none;
                        border-radius: 6px;
                        transition: background-color 0.2s ease;
                    }
                    .btn:hover {
                        background-color: #333333;
                    }
                </style>
            </head>
            <body>
                <div class="container">
                    <div class="icon">✗</div>
                    <h1>Verification Failed</h1>
                    <p>{{errorMessage}}</p>
                </div>
            </body>
            </html>
            """;
    }
}
