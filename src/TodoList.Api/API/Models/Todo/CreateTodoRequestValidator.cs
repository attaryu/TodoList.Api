using FluentValidation;
using TodoList.Api.Application.DTOs.Todo.Inputs;

namespace TodoList.Api.API.Models.Auth.Todo;

public class CreateTodoRequestValidator : AbstractValidator<CreateTodoDto>
{
    public CreateTodoRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(200)
            .WithMessage("Title length cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage("Description length cannot exceed 1000 characters.");
    }
}
