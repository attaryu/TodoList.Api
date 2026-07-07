using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.Api.Application.DTOs.Todo.Inputs;
using TodoList.Api.Application.DTOs.Todo.Outputs;
using TodoList.Api.Application.Interfaces.Services;
using TodoList.Api.Shared.Presentation.Helpers;

namespace TodoList.Api.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TodoController(ITodoService todoService) : ControllerBase
{
    private readonly ITodoService _todoService = todoService;

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!int.TryParse(userIdClaim, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user identity.");
        }
        return userId;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<TodoResultDto>>>> GetTodos()
    {
        var userId = GetCurrentUserId();
        var todos = await _todoService.GetAllByUserIdAsync(userId);
        return Ok(ApiResponseHelper.Success(todos, "Todos retrieved successfully."));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TodoResultDto>>> GetTodo(int id)
    {
        var userId = GetCurrentUserId();
        var todo = await _todoService.GetByIdAsync(id, userId);

        if (todo == null)
        {
            return NotFound(
                ApiResponseHelper.Error(404, "Todo not found", $"No Todo Item with ID {id}")
            );
        }

        return Ok(ApiResponseHelper.Success(todo, "Todo retrieved successfully."));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TodoResultDto>>> CreateTodo(CreateTodoDto todoDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var createdTodo = await _todoService.CreateAsync(todoDto, userId);
            var response = ApiResponseHelper.Success(createdTodo, "Todo created successfully.");
            return CreatedAtAction(nameof(GetTodo), new { id = createdTodo.Id }, response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponseHelper.Error(400, "Failed to create Todo", ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<TodoResultDto>>> UpdateTodo(
        int id,
        UpdateTodoDto todoDto
    )
    {
        var userId = GetCurrentUserId();
        var updatedTodo = await _todoService.UpdateAsync(id, todoDto, userId);

        if (updatedTodo == null)
        {
            return NotFound(
                ApiResponseHelper.Error(404, "Todo not found", $"No Todo Item with ID {id}")
            );
        }

        return Ok(ApiResponseHelper.Success(updatedTodo, "Todo updated successfully."));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteTodo(int id)
    {
        var userId = GetCurrentUserId();
        var success = await _todoService.DeleteAsync(id, userId);
        if (!success)
        {
            return NotFound(
                ApiResponseHelper.Error(404, "Todo not found", $"No Todo Item with ID {id}")
            );
        }

        return Ok(ApiResponseHelper.Success<object?>(null, "Todo deleted successfully."));
    }

    [HttpPatch("{id}/toggle")]
    public async Task<ActionResult<ApiResponse<TodoResultDto>>> ToggleTodo(int id)
    {
        var userId = GetCurrentUserId();
        var todo = await _todoService.ToggleAsync(id, userId);
        if (todo == null)
        {
            return NotFound(
                ApiResponseHelper.Error(404, "Todo not found", $"No Todo Item with ID {id}")
            );
        }

        return Ok(ApiResponseHelper.Success(todo, "Todo status updated successfully."));
    }
}
