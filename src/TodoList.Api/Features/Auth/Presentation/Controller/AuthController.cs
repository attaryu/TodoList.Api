using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.Api.Features.Auth.Core.DTOs.Inputs;
using TodoList.Api.Features.Auth.Core.DTOs.Outputs;
using TodoList.Api.Features.Auth.Core.UseCases;
using TodoList.Api.Features.Auth.Presentation.DTOs.Outputs;
using TodoList.Api.Shared.Helpers.Swagger.Attributes;
using TodoList.Api.Shared.Presentation.Helpers;

namespace TodoList.Api.Features.Auth.Presentation.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    RegisterUserUseCase registerUserUseCase,
    LoginUserUseCase loginUserUseCase,
    RefreshTokenUseCase refreshTokenUseCase,
    LogoutUserUseCase logoutUserUseCase,
    SendEmailVerificationUseCase sendEmailVerificationUseCase,
    VerifyEmailUseCase verifyEmailUseCase,
    GetMeUseCase getMeUseCase,
    IConfiguration configuration
) : ControllerBase
{
    private readonly RegisterUserUseCase _registerUserUseCase = registerUserUseCase;
    private readonly LoginUserUseCase _loginUserUseCase = loginUserUseCase;
    private readonly RefreshTokenUseCase _refreshTokenUseCase = refreshTokenUseCase;
    private readonly LogoutUserUseCase _logoutUserUseCase = logoutUserUseCase;
    private readonly SendEmailVerificationUseCase _sendEmailVerificationUseCase =
        sendEmailVerificationUseCase;
    private readonly VerifyEmailUseCase _verifyEmailUseCase = verifyEmailUseCase;
    private readonly GetMeUseCase _getMeUseCase = getMeUseCase;
    private readonly IConfiguration _configuration = configuration;
    private readonly string RefreshTokenCookieName = "refreshToken";

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<UserResultDto?>>> Register(RegisterDto registerDto)
    {
        try
        {
            var user = await _registerUserUseCase.ExecuteAsync(registerDto);
            return Ok(ApiResponseHelper.Success(user, "User registered successfully."));
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponseHelper.Error(400, "Validation failed", ex.Message));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponseHelper.Error(400, "Registration failed", ex.Message));
        }
    }

    [HttpPost("login")]
    [SwaggerResponseHeader(
        "Set-Cookie",
        "The new refresh token cookie.",
        "refreshToken=[Token]; Path=/; HttpOnly; Secure; SameSite=Strict"
    )]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login(LoginDto loginDto)
    {
        try
        {
            var authResponse = await _loginUserUseCase.ExecuteAsync(loginDto);

            AppendRefreshTokenToCookie(authResponse.RefreshToken);
            return Ok(
                ApiResponseHelper.Success(
                    new AuthResponseDto(
                        User: authResponse.User,
                        AccessToken: authResponse.AccessToken,
                        AccessTokenExpiresAt: authResponse.AccessTokenExpiresAt
                    ),
                    "Logged in successfully."
                )
            );
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponseHelper.Error(401, "Unauthorized", ex.Message));
        }
    }

    [HttpPost("refresh")]
    [SwaggerRequestCookie("refreshToken", "The refresh token cookie.", true)]
    [SwaggerResponseHeader(
        "Set-Cookie",
        "The new refresh token cookie.",
        "refreshToken=[Token]; Path=/; HttpOnly; Secure; SameSite=Strict"
    )]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Refresh()
    {
        try
        {
            var refreshToken = Request.Cookies[RefreshTokenCookieName];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(
                    ApiResponseHelper.Error(401, "Unauthorized", "Refresh token is missing.")
                );
            }

            var authResponse = await _refreshTokenUseCase.ExecuteAsync(refreshToken);

            AppendRefreshTokenToCookie(authResponse.RefreshToken);
            return Ok(
                ApiResponseHelper.Success(
                    new AuthResponseDto(
                        User: authResponse.User,
                        AccessToken: authResponse.AccessToken,
                        AccessTokenExpiresAt: authResponse.AccessTokenExpiresAt
                    ),
                    "Token refreshed successfully."
                )
            );
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponseHelper.Error(400, "Validation failed", ex.Message));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponseHelper.Error(401, "Unauthorized", ex.Message));
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse<object?>>> Logout()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(
                    ApiResponseHelper.Error(401, "Unauthorized", "Invalid user identity.")
                );
            }

            await _logoutUserUseCase.ExecuteAsync(userId);

            Response.Cookies.Delete(RefreshTokenCookieName);
            return Ok(ApiResponseHelper.Success<object?>(null, "Logged out successfully."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponseHelper.Error(401, "Unauthorized", ex.Message));
        }
    }

    [Authorize]
    [HttpPost("send-verification-email")]
    public async Task<ActionResult<ApiResponse<object?>>> SendVerificationEmail()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(
                    ApiResponseHelper.Error(401, "Unauthorized", "Invalid user identity.")
                );
            }

            await _sendEmailVerificationUseCase.ExecuteAsync(userId);
            return Ok(
                ApiResponseHelper.Success<object?>(null, "Verification email sent successfully.")
            );
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ApiResponseHelper.Error(400, "Request failed", ex.Message));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponseHelper.Error(401, "Unauthorized", ex.Message));
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<ApiResponse<UserResultDto>>> GetMe()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(
                    ApiResponseHelper.Error(401, "Unauthorized", "Invalid user identity.")
                );
            }

            var userDto = await _getMeUseCase.ExecuteAsync(userId);
            return Ok(ApiResponseHelper.Success(userDto, "User profile retrieved successfully."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponseHelper.Error(401, "Unauthorized", ex.Message));
        }
    }

    [AllowAnonymous]
    [HttpGet("/verify-email")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string token)
    {
        var htmlContent = await _verifyEmailUseCase.ExecuteAsync(token);
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
