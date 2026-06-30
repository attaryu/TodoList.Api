using TodoList.Api.Shared.Domain.Repositories;
using TodoList.Api.Shared.Domain.Providers;
using TodoList.Api.Features.Auth.Core.Repositories;
using TodoList.Api.Features.Auth.Core.DTOs.Outputs;
using TodoList.Api.Features.Auth.Core.DTOs.Inputs;
using TodoList.Api.Features.Auth.Core.Providers;

namespace TodoList.Api.Features.Auth.Core.UseCases;

public class LoginUserUseCase(
    IUserRepository userRepository, 
    ITokenProvider TokenProvider, 
    IUnitOfWork unitOfWork,
    IHasherProvider hasherProvider)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ITokenProvider _TokenProvider = TokenProvider;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IHasherProvider _hasherProvider = hasherProvider;

    public async Task<AuthResultDto> ExecuteAsync(LoginDto loginDto)
    {
        var normalizedEmail = loginDto.Email.ToLower().Trim();
        var user = await _userRepository.GetByEmailAsync(normalizedEmail);

        if (user == null || !_hasherProvider.Verify(loginDto.Password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid email or password.");
        }

        var (accessToken, accessTokenExpiresAt) = _TokenProvider.GenerateAccessToken(user);
        var (refreshToken, refreshTokenExpiresAt) = _TokenProvider.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAt = refreshTokenExpiresAt;
        user.UpdatedAt = DateTime.UtcNow;

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return new (
            User: new (user.Id, user.Fullname, user.Email),
            AccessToken: accessToken,
            RefreshToken: refreshToken,
            AccessTokenExpiresAt: accessTokenExpiresAt);
    }
}
