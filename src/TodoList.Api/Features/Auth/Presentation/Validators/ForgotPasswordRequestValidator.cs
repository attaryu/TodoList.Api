using FluentValidation;
using TodoList.Api.Features.Auth.Core.DTOs.Inputs;

namespace TodoList.Api.Features.Auth.Presentation.Validators;

public class ForgotPasswordRequestValidator : AbstractValidator<ForgotPasswordDto>
{
    public ForgotPasswordRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .MinimumLength(5)
            .WithMessage("Email length must be at least 5 characters.")
            .MaximumLength(100)
            .WithMessage("Email length cannot exceed 100 characters.");
    }
}
