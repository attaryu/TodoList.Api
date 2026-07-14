using System.ComponentModel;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ModelContextProtocol.Server;
using TodoList.Api.Application.Interfaces.Services;

namespace TodoList.Api.Presentation.Mcp.Resources;

[McpServerResourceType]
public class TodoResources(ITodoService todoService, IHttpContextAccessor httpContextAccessor)
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

    [McpServerResource(UriTemplate = "todos://list", Name = "All Todo Items")]
    [Description("Retrieves all todo items for the authenticated user.")]
    public async Task<string> ListTodos()
    {
        var userId = GetUserId();
        var result = await _todoService.GetAllByUserIdAsync(userId);
        return System.Text.Json.JsonSerializer.Serialize(result);
    }

    [McpServerResource(UriTemplate = "todos://{id}", Name = "Todo Item Detail")]
    [Description("Retrieves details of a specific todo item by its ID.")]
    public async Task<string> GetTodo(string id)
    {
        var userId = GetUserId();
        if (!Guid.TryParse(id, out var todoId))
        {
            return "{\"error\": \"Invalid Todo ID format.\"}";
        }

        var result = await _todoService.GetByIdAsync(todoId, userId);
        return System.Text.Json.JsonSerializer.Serialize(result);
    }
}
