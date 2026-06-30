using System.ComponentModel.DataAnnotations;

namespace TodoList.Api.Features.Todo.Core.DTOs;

public class CreateTodoDto
{
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool? IsCompleted { get; set; } = false;
}
