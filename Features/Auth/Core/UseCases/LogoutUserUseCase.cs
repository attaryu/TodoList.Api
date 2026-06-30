using TodoList.Api.Shared.Domain.Repositories;
using TodoList.Api.Features.Auth.Core.Repositories;

namespace TodoList.Api.Features.Auth.Core.UseCases;

public class LogoutUserUseCase(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task ExecuteAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId) ?? throw new UnauthorizedAccessException("User not found.");

        user.RefreshToken = null;
        user.RefreshTokenExpiresAt = null;
        user.UpdatedAt = DateTime.UtcNow;

        _userRepository.Update(user);
        await _unitOfWork.SaveChangesAsync();
    }
}
