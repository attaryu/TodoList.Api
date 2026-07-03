using TodoList.Api.Features.Auth.Core.DTOs.Inputs;
using TodoList.Api.Features.Auth.Core.DTOs.Outputs;
using TodoList.Api.Features.Auth.Core.Entities;
using TodoList.Api.Features.Auth.Core.Providers;
using TodoList.Api.Features.Auth.Core.Repositories;
using TodoList.Api.Shared.Domain.Repositories;

namespace TodoList.Api.Features.Auth.Core.UseCases;

public class RegisterUserUseCase(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IHasherProvider hasherProvider
)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IHasherProvider _hasherProvider = hasherProvider;

    public async Task<UserResultDto> ExecuteAsync(RegisterDto registerDto)
    {
        var normalizedEmail = registerDto.Email.ToLower().Trim();
        var existingUser = await _userRepository.GetByEmailAsync(normalizedEmail);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        var passwordHash = _hasherProvider.HashText(registerDto.Password);

        User user = new()
        {
            Fullname = registerDto.Fullname,
            Email = normalizedEmail,
            PasswordHash = passwordHash,
        };

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return new(
            Id: user.Id,
            Fullname: user.Fullname,
            Email: user.Email,
            IsEmailVerified: user.IsEmailVerified
        );
    }
}
