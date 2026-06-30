using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.Api.Features.Auth.Core.DTOs.Inputs;
using TodoList.Api.Features.Auth.Core.DTOs.Outputs;
using TodoList.Api.Features.Auth.Core.UseCases;
using TodoList.Api.Shared.Presentation.Helpers;

namespace TodoList.Api.Features.Auth.Presentation.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    RegisterUserUseCase registerUserUseCase,
    LoginUserUseCase loginUserUseCase,
    RefreshTokenUseCase refreshTokenUseCase,
    LogoutUserUseCase logoutUserUseCase
) : ControllerBase
{
    private readonly RegisterUserUseCase _registerUserUseCase = registerUserUseCase;
    private readonly LoginUserUseCase _loginUserUseCase = loginUserUseCase;
    private readonly RefreshTokenUseCase _refreshTokenUseCase = refreshTokenUseCase;
    private readonly LogoutUserUseCase _logoutUserUseCase = logoutUserUseCase;

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<UserResultDto?>>> Register(RegisterDto registerDto)
    {
        try
        {
            var user = await _registerUserUseCase.ExecuteAsync(
                registerDto.Email,
                registerDto.Password
            );
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
    public async Task<ActionResult<ApiResponse<AuthResultDto>>> Login(LoginDto loginDto)
    {
        try
        {
            var authResponse = await _loginUserUseCase.ExecuteAsync(loginDto);
            return Ok(ApiResponseHelper.Success(authResponse, "Logged in successfully."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponseHelper.Error(401, "Unauthorized", ex.Message));
        }
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<AuthResultDto>>> Refresh(RefreshTokenDto refreshDto)
    {
        try
        {
            var authResponse = await _refreshTokenUseCase.ExecuteAsync(refreshDto);
            return Ok(ApiResponseHelper.Success(authResponse, "Token refreshed successfully."));
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
            return Ok(ApiResponseHelper.Success<object?>(null, "Logged out successfully."));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponseHelper.Error(401, "Unauthorized", ex.Message));
        }
    }
}
