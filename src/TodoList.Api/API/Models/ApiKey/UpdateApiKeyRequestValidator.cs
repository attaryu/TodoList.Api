using FluentValidation;
using TodoList.Api.Application.DTOs.ApiKey.Inputs;

namespace TodoList.Api.API.Models.ApiKey;

public class UpdateApiKeyRequestValidator : AbstractValidator<UpdateApiKeyDto>
{
    public UpdateApiKeyRequestValidator()
    {
        RuleFor(x => x.Label)
            .NotEmpty()
            .WithMessage("Label is required.")
            .MaximumLength(50)
            .WithMessage("Label length cannot exceed 50 characters.");
    }
}
