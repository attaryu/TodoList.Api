using System.ComponentModel.DataAnnotations;

namespace TodoList.Api.Features.Todo.Presentation.DTOs;

public class CreateTodoDto
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, ErrorMessage = "Title length cannot exceed 200 characters.")]
    public string Title { get; init; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Description length cannot exceed 1000 characters.")]
    public string? Description { get; init; }

    [Required(ErrorMessage = "IsCompleted status is required.")]
    public bool? IsCompleted { get; init; }
}
