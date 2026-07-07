using FluentValidation;
using TodoList.Api.Application.DTOs.Todo.Inputs;

namespace TodoList.Api.API.Models.Auth.Todo;

public class UpdateTodoRequestValidator : AbstractValidator<UpdateTodoDto>
{
    public UpdateTodoRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required.")
            .MaximumLength(200)
            .WithMessage("Title length cannot exceed 200 characters.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required.")
            .MaximumLength(1000)
            .WithMessage("Description length cannot exceed 1000 characters.");

        RuleFor(x => x.IsCompleted).NotNull().WithMessage("IsCompleted is required.");
    }
}
