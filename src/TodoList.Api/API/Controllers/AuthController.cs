using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sindika.AspNet.Request;
using Sindika.AspNet.Response;
using TodoList.Api.API.Controllers.Base;
using TodoList.Api.Application.DTOs.Auth.Inputs;
using TodoList.Api.Application.DTOs.Auth.Outputs;
using TodoList.Api.Application.Interfaces.Services;
using TodoList.Api.Common.Helpers.Swagger.Attributes;

namespace TodoList.Api.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, IConfiguration configuration)
    : BaseApiController
{
    private readonly IAuthService _authService = authService;
    private readonly IConfiguration _configuration = configuration;
    private readonly string RefreshTokenCookieName = "refreshToken";

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] BaseRequest<RegisterDto> request)
    {
        var user = await _authService.RegisterAsync(request.Data);
        return Ok(
            ResponseHelper.Success<RegisterDto>(
                user,
                "User registered successfully.",
                null,
                request
            )
        );
    }

    [HttpPost("login")]
    [SwaggerResponseHeader(
        "Set-Cookie",
        "The new refresh token cookie.",
        "refreshToken=[Token]; Path=/; HttpOnly; Secure; SameSite=Strict"
    )]
    public async Task<IActionResult> Login([FromBody] BaseRequest<LoginDto> request)
    {
        var authResponse = await _authService.LoginAsync(request.Data);

        AppendRefreshTokenToCookie(authResponse.RefreshToken);
        var responseDto = new AuthResponseDto(
            User: authResponse.User,
            AccessToken: authResponse.AccessToken,
            AccessTokenExpiresAt: authResponse.AccessTokenExpiresAt
        );
        return Ok(
            ResponseHelper.Success<LoginDto>(responseDto, "Logged in successfully.", null, request)
        );
    }

    [HttpPost("refresh")]
    [SwaggerRequestCookie("refreshToken", "The refresh token cookie.", true)]
    [SwaggerResponseHeader(
        "Set-Cookie",
        "The new refresh token cookie.",
        "refreshToken=[Token]; Path=/; HttpOnly; Secure; SameSite=Strict"
    )]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies[RefreshTokenCookieName];
        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new UnauthorizedAccessException("Refresh token is missing.");
        }

        var authResponse = await _authService.RefreshTokenAsync(refreshToken);

        AppendRefreshTokenToCookie(authResponse.RefreshToken);
        var responseDto = new AuthResponseDto(
            User: authResponse.User,
            AccessToken: authResponse.AccessToken,
            AccessTokenExpiresAt: authResponse.AccessTokenExpiresAt
        );
        return Ok(ResponseHelper.Success<object>(responseDto, "Token refreshed successfully."));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var userId = GetCurrentUserId();

        await _authService.LogoutAsync(userId);

        Response.Cookies.Delete(RefreshTokenCookieName);
        return Ok(ResponseHelper.Success<object>(null, "Logged out successfully."));
    }

    [Authorize]
    [HttpPost("send-verification-email")]
    public async Task<IActionResult> SendVerificationEmail()
    {
        var userId = GetCurrentUserId();

        await _authService.SendEmailVerificationAsync(userId);
        return Ok(ResponseHelper.Success<object>(null, "Verification email sent successfully."));
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMe()
    {
        var userId = GetCurrentUserId();

        var userDto = await _authService.GetMeAsync(userId);
        return Ok(ResponseHelper.Success<object>(userDto, "User profile retrieved successfully."));
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(
        [FromBody] BaseRequest<ForgotPasswordDto> request
    )
    {
        await _authService.ForgotPasswordAsync(request.Data);
        return Ok(
            ResponseHelper.Success<ForgotPasswordDto>(
                null,
                "If the email is registered, a password reset link has been sent.",
                null,
                request
            )
        );
    }

    [AllowAnonymous]
    [HttpGet("/reset-password")]
    public async Task<IActionResult> GetResetPasswordPage([FromQuery] string token)
    {
        var htmlContent = await _authService.GetResetPasswordPageAsync(token);
        return Content(htmlContent, "text/html");
    }

    [AllowAnonymous]
    [HttpPost("/reset-password")]
    public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordDto resetPasswordDto)
    {
        var htmlContent = await _authService.ResetPasswordAsync(resetPasswordDto);
        return Content(htmlContent, "text/html");
    }

    [AllowAnonymous]
    [HttpGet("/verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        var htmlContent = await _authService.VerifyEmailAsync(token);
        return Content(htmlContent, "text/html");
    }

    // helper

    private void AppendRefreshTokenToCookie(string refreshToken)
    {
        string expireInDays =
            _configuration["JWT_REFRESH_EXPIRY_DAYS"]
            ?? throw new InvalidOperationException(
                "JWT_REFRESH_EXPIRY_DAYS configuration is missing."
            );

        CookieOptions cookieOptions = new()
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(int.Parse(expireInDays)),
        };

        Response.Cookies.Append(RefreshTokenCookieName, refreshToken, cookieOptions);
    }
}
