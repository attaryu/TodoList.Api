using TodoList.Api.Application.DTOs.Auth.Inputs;
using TodoList.Api.Application.DTOs.Auth.Outputs;

namespace TodoList.Api.Application.Interfaces.Services;

public interface IAuthService
{
    Task<UserResultDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResultDto> LoginAsync(LoginDto loginDto);
    Task<AuthResultDto> RefreshTokenAsync(string refreshToken);
    Task LogoutAsync(Guid userId);
    Task SendEmailVerificationAsync(Guid userId);
    Task<string> VerifyEmailAsync(string token);
    Task<UserResultDto> GetMeAsync(Guid userId);
    Task ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
    Task<string> GetResetPasswordPageAsync(string token);
    Task<string> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
}
