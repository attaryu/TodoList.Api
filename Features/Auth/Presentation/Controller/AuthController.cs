using Microsoft.AspNetCore.Mvc;
using TodoList.Api.Features.Auth.Core.UseCases;
using TodoList.Api.Features.Auth.Core.DTOs;
using TodoList.Api.Shared.Presentation.Helpers;
using AutoMapper;
using TodoList.Api.Features.Auth.Core.Entities;

namespace TodoList.Api.Features.Auth.Presentation.Controller;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    RegisterUserUseCase registerUserUseCase,
    LoginUserUseCase loginUserUseCase,
    RefreshTokenUseCase refreshTokenUseCase,
    IMapper mapper) : ControllerBase
{
    private readonly RegisterUserUseCase _registerUserUseCase = registerUserUseCase;
    private readonly LoginUserUseCase _loginUserUseCase = loginUserUseCase;
    private readonly RefreshTokenUseCase _refreshTokenUseCase = refreshTokenUseCase;
    private readonly IMapper _mapper = mapper;

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<UserResponseDto?>>> Register(RegisterDto registerDto)
    {
        try
        {
            var user = await _registerUserUseCase.ExecuteAsync(registerDto.Email, registerDto.Password);
            var data = _mapper.Map<UserResponseDto>(user);
            
            return Ok(ApiResponseHelper.Success(data, "User registered successfully."));
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
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login(LoginDto loginDto)
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
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Refresh(RefreshTokenDto refreshDto)
    {
        try
        {
            var authResponse = await _refreshTokenUseCase.ExecuteAsync(refreshDto.RefreshToken);
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
}
