using System.ComponentModel;
using System.Security.Claims;
using ModelContextProtocol.Server;
using TodoList.Api.Application.DTOs.Todo.Inputs;
using TodoList.Api.Application.Interfaces.Services;

namespace TodoList.Api.Presentation.Mcp.Tools;

[McpServerToolType]
public class TodoTools(ITodoService todoService, IHttpContextAccessor httpContextAccessor)
{
    private readonly ITodoService _todoService = todoService;
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    private Guid GetUserId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new InvalidOperationException("HttpContext is not available.");
        }

        var userIdClaim = httpContext.User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        return userId;
    }

    [McpServerTool, Description("Creates a new todo item.")]
    public async Task<string> CreateTodo(
        [Description("The title of the todo item.")] string title,
        [Description("The optional description of the todo item.")] string? description
    )
    {
        var userId = GetUserId();
        var dto = new CreateTodoDto(title, description, false);
        var result = await _todoService.CreateAsync(dto, userId);
        return System.Text.Json.JsonSerializer.Serialize(result);
    }

    [
        McpServerTool,
        Description(
            "Updates an existing todo item. Pass only fields that should be updated. Description and IsCompleted are optional but will be updated if provided."
        )
    ]
    public async Task<string> UpdateTodo(
        [Description("The ID of the todo item to update.")] string id,
        [Description("The new title for the todo item.")] string title,
        [Description("The new description for the todo item.")] string description,
        [Description("The completion status of the todo item.")] bool isCompleted
    )
    {
        var userId = GetUserId();
        if (!Guid.TryParse(id, out var todoId))
        {
            return "{\"error\": \"Invalid Todo ID format.\"}";
        }

        var dto = new UpdateTodoDto(title, description, isCompleted);
        var result = await _todoService.UpdateAsync(todoId, dto, userId);

        return System.Text.Json.JsonSerializer.Serialize(result);
    }

    [McpServerTool, Description("Deletes a todo item (soft delete).")]
    public async Task<string> DeleteTodo(
        [Description("The ID of the todo item to delete.")] string id
    )
    {
        var userId = GetUserId();
        if (!Guid.TryParse(id, out var todoId))
        {
            return "{\"error\": \"Invalid Todo ID format.\"}";
        }

        var deleted = await _todoService.DeleteAsync(todoId, userId);
        return System.Text.Json.JsonSerializer.Serialize(new { success = deleted });
    }
}
