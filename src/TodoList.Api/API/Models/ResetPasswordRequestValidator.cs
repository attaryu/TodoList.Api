using FluentValidation;
using TodoList.Api.Application.DTOs.Auth.Inputs;

namespace TodoList.Api.API.Models;

public class ResetPasswordRequestValidator : AbstractValidator<ResetPasswordDto>
{
    public ResetPasswordRequestValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage("Token is required.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long.")
            .MaximumLength(100)
            .WithMessage("Password length cannot exceed 100 characters.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Confirm password is required.")
            .Equal(x => x.Password)
            .WithMessage("Passwords do not match. Please try again.");
    }
}
