using TodoList.Api.Features.Auth.Core.DTOs.Outputs;
using TodoList.Api.Features.Auth.Core.Repositories;

namespace TodoList.Api.Features.Auth.Core.UseCases;

public class GetMeUseCase(IUserRepository userRepository)
{
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<UserResultDto> ExecuteAsync(int userId)
    {
        var user =
            await _userRepository.GetByIdAsync(userId)
            ?? throw new UnauthorizedAccessException("User not found.");

        return new(
            Id: user.Id,
            Fullname: user.Fullname,
            Email: user.Email,
            IsEmailVerified: user.IsEmailVerified
        );
    }
}
