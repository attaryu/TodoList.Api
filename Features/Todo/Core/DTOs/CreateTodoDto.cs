using System.ComponentModel.DataAnnotations;

namespace TodoList.Api.Features.Todo.Core.DTOs;

public class CreateTodoDto
{
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public bool IsCompleted { get; init; }
}
