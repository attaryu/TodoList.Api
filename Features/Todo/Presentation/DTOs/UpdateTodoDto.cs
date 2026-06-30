using System.ComponentModel.DataAnnotations;

namespace TodoList.Api.Features.Todo.Presentation.DTOs;

public class UpdateTodoDto
{
    [Required(ErrorMessage = "Title is required.")]
    [StringLength(200, ErrorMessage = "Title length cannot exceed 200 characters.")]
    public string Title { get; init; } = string.Empty;

    [Required(ErrorMessage = "Description is required.")]
    [StringLength(1000, ErrorMessage = "Description length cannot exceed 1000 characters.")]
    public string Description { get; init; } = string.Empty;

    [Required(ErrorMessage = "IsCompleted status is required.")]
    public bool? IsCompleted { get; init; }
}
