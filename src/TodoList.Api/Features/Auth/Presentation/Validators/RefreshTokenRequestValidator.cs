using FluentValidation;
using TodoList.Api.Features.Auth.Core.DTOs.Inputs;

namespace TodoList.Api.Features.Auth.Presentation.Validators;

public class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenDto>
{
    public RefreshTokenRequestValidator()
    {
        RuleFor(x => x.RefreshToken).NotEmpty().WithMessage("Refresh token is required.");
    }
}
