using TodoList.Api.Shared.Domain.Repositories;
using TodoList.Api.Features.Auth.Core.Entities;
using TodoList.Api.Features.Auth.Core.Repositories;
using TodoList.Api.Features.Auth.Core.Providers;
using TodoList.Api.Features.Auth.Core.DTOs.Outputs;

namespace TodoList.Api.Features.Auth.Core.UseCases;

public class RegisterUserUseCase(IUserRepository userRepository, IUnitOfWork unitOfWork, IHasherProvider hasherProvider)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IHasherProvider _hasherProvider = hasherProvider;

    public async Task<UserResultDto> ExecuteAsync(string email, string password)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email is required.");
        }

        if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
        {
            throw new ArgumentException("Password must be at least 6 characters long.");
        }

        var normalizedEmail = email.ToLower().Trim();
        var existingUser = await _userRepository.GetByEmailAsync(normalizedEmail);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email is already registered.");
        }

        var passwordHash = _hasherProvider.HashText(password);

        var user = new User
        {
            Email = normalizedEmail,
            PasswordHash = passwordHash
        };

        await _userRepository.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return new(
            Id: user.Id,
            Fullname: user.Fullname,
            Email: user.Email);
    }
}
