using TodoList.Api.Features.Auth.Core.Repositories;

namespace TodoList.Api.Features.Auth.Core.UseCases;

public class GetResetPasswordPageUseCase(IPasswordResetTokenRepository passwordResetTokenRepository)
{
    private readonly IPasswordResetTokenRepository _passwordResetTokenRepository =
        passwordResetTokenRepository;

    public async Task<string> ExecuteAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            return GetErrorHtml("Reset password token cannot be empty or invalid.");
        }

        var passwordResetToken = await _passwordResetTokenRepository.GetByTokenAsync(token);

        if (passwordResetToken == null)
        {
            return GetErrorHtml(
                "Reset password token is invalid or not found. Please request a new link."
            );
        }

        if (passwordResetToken.ExpiresAt < DateTime.UtcNow)
        {
            return GetErrorHtml("Reset password token has expired. Please request a new link.");
        }

        return GetFormHtml(token);
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
                    <h1>Link Invalid</h1>
                    <p>{{errorMessage}}</p>
                </div>
            </body>
            </html>
            """;
    }
}
