using TodoList.Api.Features.Auth.Core.Entities;

namespace TodoList.Api.Features.Todo.Core.Entities;

public class TodoItem
{
	public int Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string? Description { get; set; }
	public bool IsCompleted { get; set; }
	public DateTime? CompletedAt { get; set; }

	public int UserId { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
	
	public User User { get; set; } = null!;
}
