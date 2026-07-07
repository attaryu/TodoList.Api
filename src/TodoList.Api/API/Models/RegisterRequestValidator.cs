using FluentValidation;
using TodoList.Api.Application.DTOs.Auth.Inputs;

namespace TodoList.Api.API.Models;

public class RegisterRequestValidator : AbstractValidator<RegisterDto>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Fullname)
            .NotEmpty()
            .WithMessage("Fullname is required.")
            .MinimumLength(2)
            .WithMessage("Fullname length must be at least 2 characters.")
            .MaximumLength(50)
            .WithMessage("Fullname length cannot exceed 50 characters.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .MinimumLength(5)
            .WithMessage("Email length must be at least 5 characters.")
            .MaximumLength(100)
            .WithMessage("Email length cannot exceed 100 characters.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(6)
            .WithMessage("Password length must be at least 6 characters.")
            .MaximumLength(100)
            .WithMessage("Password length cannot exceed 100 characters.");
    }
}
