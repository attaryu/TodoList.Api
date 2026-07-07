using TodoList.Api.Application.DTOs.Auth.Inputs;
using TodoList.Api.Application.DTOs.Auth.Outputs;

namespace TodoList.Api.Application.Interfaces.Services;

public interface IAuthService
{
    Task<UserResultDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResultDto> LoginAsync(LoginDto loginDto);
    Task<AuthResultDto> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(int userId);
    Task SendEmailVerificationAsync(int userId);
    Task<string> VerifyEmailAsync(string token);
    Task<UserResultDto> GetMeAsync(int userId);
    Task ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
    Task<string> GetResetPasswordPageAsync(string token);
    Task<string> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
}
