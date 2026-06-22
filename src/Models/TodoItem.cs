namespace TodoList.Api.src.Models;

/// <summary>
/// Represts a todo item
/// </summary>
public class TodoItem
{
	public int Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string? Description { get; set; }
	public bool IsCompleted { get; set; }
	public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	public DateTime CompletedAt { get; set; }
}
