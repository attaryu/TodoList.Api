using TodoList.Api.Features.Auth.Core.DTOs.Inputs;
using TodoList.Api.Features.Auth.Core.DTOs.Outputs;
using TodoList.Api.Features.Auth.Core.Repositories;
using TodoList.Api.Shared.Domain.Providers;
using TodoList.Api.Shared.Domain.Repositories;

namespace TodoList.Api.Features.Auth.Core.UseCases;

public class RefreshTokenUseCase(
    IUserRepository userRepository,
    ITokenProvider TokenProvider,
    IUnitOfWork unitOfWork
)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ITokenProvider _TokenProvider = TokenProvider;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<AuthResultDto> ExecuteAsync(RefreshTokenDto refreshTokenDto)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(refreshTokenDto.RefreshToken);
        if (user == null || user.RefreshTokenExpiresAt < DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        }

        var (newAccessToken, newAccessTokenExpiresAt) = _TokenProvider.GenerateAccessToken(user);
        var (newRefreshToken, newRefreshTokenExpiresAt) = _TokenProvider.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiresAt = newRefreshTokenExpiresAt;
        user.UpdatedAt = DateTime.UtcNow;

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return new(
            User: new(Id: user.Id, Fullname: user.Fullname, Email: user.Email),
            AccessToken: newAccessToken,
            RefreshToken: newRefreshToken,
            AccessTokenExpiresAt: newAccessTokenExpiresAt
        );
    }
}
