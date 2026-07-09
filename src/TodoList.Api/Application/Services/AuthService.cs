using System.Security.Cryptography;
using Mapster;
using MassTransit;
using TodoList.Api.Application.DTOs.Auth.Inputs;
using TodoList.Api.Application.DTOs.Auth.Outputs;
using TodoList.Api.Application.Interfaces.Repositories;
using TodoList.Api.Application.Interfaces.Services;
using TodoList.Api.Domain.Entities;
using TodoList.Contracts.Commands;

namespace TodoList.Api.Application.Services;

public class AuthService(
    IUserRepository userRepository,
    IPasswordResetTokenRepository passwordResetTokenRepository,
    IEmailVerificationRepository emailVerificationRepository,
    IUnitOfWork unitOfWork,
    ITokenProvider tokenProvider,
    IHasherProvider hasherProvider,
    ISendEndpointProvider sendEndpointProvider,
    IConfiguration configuration
) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository =
        passwordResetTokenRepository;
    private readonly IEmailVerificationRepository _emailVerificationRepository =
        emailVerificationRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ITokenProvider _tokenProvider = tokenProvider;
    private readonly IHasherProvider _hasherProvider = hasherProvider;
    private readonly ISendEndpointProvider _sendEndpointProvider = sendEndpointProvider;
    private readonly IConfiguration _configuration = configuration;

    public async Task<UserResultDto> RegisterAsync(RegisterDto registerDto)
    {
        var normalizedEmail = registerDto.Email.ToLower().Trim();
        var existingUser = await _userRepository.GetByEmailAsync(normalizedEmail);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        var passwordHash = _hasherProvider.HashText(registerDto.Password);

        User user = new()
        {
            Fullname = registerDto.Fullname,
            Email = normalizedEmail,
            PasswordHash = passwordHash,
        };

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return user.Adapt<UserResultDto>();
    }

    public async Task<AuthResultDto> LoginAsync(LoginDto loginDto)
    {
        var normalizedEmail = loginDto.Email.ToLower().Trim();
        var user = await _userRepository.GetByEmailAsync(normalizedEmail);

        if (user == null || !_hasherProvider.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var (accessToken, accessTokenExpiresAt) = _tokenProvider.GenerateAccessToken(user);
        var (refreshToken, refreshTokenExpiresAt) = _tokenProvider.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAt = refreshTokenExpiresAt;

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return new(
            User: user.Adapt<UserResultDto>(),
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            AccessTokenExpiresAt: accessTokenExpiresAt
        );
    }

    public async Task<AuthResultDto> RefreshTokenAsync(string refreshToken)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(refreshToken);
        if (user == null || user.RefreshTokenExpiresAt < DateTimeOffset.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        }

        var (newAccessToken, newAccessTokenExpiresAt) = _tokenProvider.GenerateAccessToken(user);
        var (newRefreshToken, newRefreshTokenExpiresAt) = _tokenProvider.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiresAt = newRefreshTokenExpiresAt;

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return new(
            User: user.Adapt<UserResultDto>(),
            AccessToken: newAccessToken,
            RefreshToken: newRefreshToken,
            AccessTokenExpiresAt: newAccessTokenExpiresAt
        );
    }

    public async Task LogoutAsync(Guid userId)
    {
        var user =
            await _userRepository.GetByIdAsync(userId)
            ?? throw new UnauthorizedAccessException("User not found.");

        user.RefreshToken = null;
        user.RefreshTokenExpiresAt = null;

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task SendEmailVerificationAsync(Guid userId)
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
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(10),
        };

        await _emailVerificationRepository.AddAsync(verification);
        await _unitOfWork.SaveChangesAsync();

        var domain = _configuration["DOMAIN"];

        // Construct HTML email body
        var emailBody = $"""
            <h1>Verify Your Email</h1>
            <p>Please use click the button below to verify your email address:</p>
            <button style="padding: 10px 20px; font-size: 16px; background-color: black; color: white; border: none; border-radius: 5px; cursor: pointer;">
                <a href="{domain}/verify-email?token={verification.Token}" style="color: white; text-decoration: none;" target="_blank">
                    Verify Email
                </a>
            </button>
            <p>This link will expire in 10 minutes from {DateTimeOffset.UtcNow:dd MMM yyyy HH:mm:ss} UTC.</p>
            """;

        // Send message to mapped endpoint
        await _sendEndpointProvider.Send(
            new SendEmailNotification(user.Email, "Verify Your Email Address", emailBody)
        );
    }

    public async Task<string> VerifyEmailAsync(string token)
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

        if (verification.ExpiresAt < DateTimeOffset.UtcNow)
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
            _userRepository.Update(user);
        }

        await _emailVerificationRepository.DeleteByUserIdAsync(user.Id);
        await _unitOfWork.SaveChangesAsync();

        return GetSuccessHtml(user.Fullname, user.Email);
    }

    public async Task<UserResultDto> GetMeAsync(Guid userId)
    {
        var user =
            await _userRepository.GetByIdAsync(userId)
            ?? throw new UnauthorizedAccessException("User not found.");

        return user.Adapt<UserResultDto>();
    }

    public async Task ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
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
            ExpiresAt = DateTimeOffset.UtcNow.AddMinutes(15),
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
            <p>This link will expire in 15 minutes from {DateTimeOffset.UtcNow:dd MMM yyyy HH:mm:ss} UTC.</p>
            """;

        await _sendEndpointProvider.Send(
            new SendEmailNotification(user.Email, "Reset Your Password", emailBody)
        );
    }

    public async Task<string> GetResetPasswordPageAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return GetResetPasswordPageErrorHtml(
                "Reset password token cannot be empty or invalid."
            );
        }

        var passwordResetToken = await _passwordResetTokenRepository.GetByTokenAsync(token);

        if (passwordResetToken == null)
        {
            return GetResetPasswordPageErrorHtml(
                "Reset password token is invalid or not found. Please request a new link."
            );
        }

        if (passwordResetToken.ExpiresAt < DateTimeOffset.UtcNow)
        {
            return GetResetPasswordPageErrorHtml(
                "Reset password token has expired. Please request a new link."
            );
        }

        return GetFormHtml(token);
    }

    public async Task<string> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        var passwordResetToken = await _passwordResetTokenRepository.GetByTokenAsync(
            resetPasswordDto.Token
        );

        if (passwordResetToken == null)
        {
            return GetResetPasswordErrorHtml("Reset password token is invalid or not found.");
        }

        if (passwordResetToken.ExpiresAt < DateTimeOffset.UtcNow)
        {
            await _passwordResetTokenRepository.DeleteByUserIdAsync(passwordResetToken.UserId);
            await _unitOfWork.SaveChangesAsync();
            return GetResetPasswordErrorHtml(
                "Reset password token has expired. Please request a new link."
            );
        }

        var user = passwordResetToken.User;
        if (user == null)
        {
            return GetResetPasswordErrorHtml("User associated with this token is not found.");
        }

        user.PasswordHash = _hasherProvider.HashText(resetPasswordDto.Password);
        user.RefreshToken = null;
        user.RefreshTokenExpiresAt = null;

        _userRepository.Update(user);
        await _passwordResetTokenRepository.DeleteByUserIdAsync(user.Id);
        await _unitOfWork.SaveChangesAsync();

        return GetResetPasswordSuccessHtml(user.Fullname);
    }

    #region Helper Methods (HTML Pages)

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

    private static string GetFormHtml(string token)
    {
        return $$"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport" content="width=device-width, initial-scale=1.0">
                <title>Reset Your Password</title>
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
                        max-width: 400px;
                        width: 100%;
                    }
                    h1 {
                        font-size: 24px;
                        margin-bottom: 20px;
                        color: #2c3e50;
                        text-align: center;
                    }
                    p {
                        font-size: 14px;
                        line-height: 1.5;
                        color: #7f8c8d;
                        margin-bottom: 25px;
                        text-align: center;
                    }
                    .form-group {
                        margin-bottom: 20px;
                    }
                    label {
                        display: block;
                        font-size: 14px;
                        font-weight: 600;
                        margin-bottom: 8px;
                        color: #2c3e50;
                    }
                    input[type="password"] {
                        width: 100%;
                        padding: 10px;
                        font-size: 14px;
                        border: 1px solid #dcdde1;
                        border-radius: 6px;
                        box-sizing: border-box;
                    }
                    input[type="password"]:focus {
                        outline: none;
                        border-color: #3498db;
                    }
                    .btn {
                        display: block;
                        width: 100%;
                        padding: 12px;
                        font-size: 16px;
                        font-weight: 600;
                        background-color: #000000;
                        color: #ffffff;
                        border: none;
                        border-radius: 6px;
                        cursor: pointer;
                        text-align: center;
                        transition: background-color 0.2s ease;
                    }
                    .btn:hover {
                        background-color: #333333;
                    }
                    .error-box {
                        color: #e74c3c;
                        background-color: #fadbd8;
                        border: 1px solid #f5b7b1;
                        padding: 10px;
                        border-radius: 6px;
                        font-size: 14px;
                        margin-bottom: 20px;
                        text-align: center;
                        display: none;
                    }
                </style>
            </head>
            <body>
                <div class="container">
                    <h1>Reset Password</h1>
                    <p>Enter your new password below.</p>
                    <div id="client-error" class="error-box"></div>
                    <form method="POST" action="/reset-password">
                        <input type="hidden" name="Token" value="{{token}}" />
                        <div class="form-group">
                            <label for="password">New Password</label>
                            <input type="password" id="password" name="Password" required minlength="6" />
                        </div>
                        <div class="form-group">
                            <label for="confirm-password">Confirm Password</label>
                            <input type="password" id="confirm-password" name="ConfirmPassword" required minlength="6" />
                        </div>
                        <button type="submit" class="btn">Reset Password</button>
                    </form>
                </div>
                <script>
                    const form = document.querySelector('form');
                    const password = document.getElementById('password');
                    const confirmPassword = document.getElementById('confirm-password');
                    const errorBox = document.getElementById('client-error');

                    form.addEventListener('submit', function(e) {
                        errorBox.style.display = 'none';
                        errorBox.textContent = '';
                        
                        if (password.value.length < 6) {
                            e.preventDefault();
                            errorBox.textContent = 'Password must be at least 6 characters long.';
                            errorBox.style.display = 'block';
                            return;
                        }

                        if (password.value !== confirmPassword.value) {
                            e.preventDefault();
                            errorBox.textContent = 'Passwords do not match. Please try again.';
                            errorBox.style.display = 'block';
                            return;
                        }
                    });
                </script>
            </body>
            </html>
            """;
    }

    private static string GetResetPasswordPageErrorHtml(string errorMessage)
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
                    <h1>Link Invalid</h1>
                    <p>{{errorMessage}}</p>
                </div>
            </body>
            </html>
            """;
    }

    private static string GetResetPasswordSuccessHtml(string fullname)
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

    private static string GetResetPasswordErrorHtml(string errorMessage)
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

    #endregion
}
