using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.Api.Features.Todo.Core.DTOs.Inputs;
using TodoList.Api.Features.Todo.Core.DTOs.Outputs;
using TodoList.Api.Features.Todo.Core.UseCases;
using TodoList.Api.Shared.Presentation.Helpers;

namespace TodoList.Api.Features.Todo.Presentation.Controller;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TodoController(
    GetTodoUseCase getTodoUseCase,
    GetTodosUseCase getTodosUseCase,
    CreateTodoUseCase CreateTodoUseCase,
    UpdateTodoUseCase updateTodoUseCase,
    DeleteTodoUseCase deleteTodoUseCase,
    ToggleTodoUseCase toggleTodoUseCase
) : ControllerBase
{
    private readonly GetTodoUseCase _getTodoUseCase = getTodoUseCase;
    private readonly GetTodosUseCase _getTodosUseCase = getTodosUseCase;
    private readonly CreateTodoUseCase _CreateTodoUseCase = CreateTodoUseCase;
    private readonly UpdateTodoUseCase _updateTodoUseCase = updateTodoUseCase;
    private readonly DeleteTodoUseCase _deleteTodoUseCase = deleteTodoUseCase;
    private readonly ToggleTodoUseCase _toggleTodoUseCase = toggleTodoUseCase;

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
        var todos = await _getTodosUseCase.ExecuteAsync(userId);
        return Ok(ApiResponseHelper.Success(todos, "Todos retrieved successfully."));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TodoResultDto>>> GetTodo(int id)
    {
        var userId = GetCurrentUserId();
        var todo = await _getTodoUseCase.ExecuteAsync(id, userId);

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
            var createdTodo = await _CreateTodoUseCase.ExecuteAsync(todoDto, userId);
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
        var updatedTodo = await _updateTodoUseCase.ExecuteAsync(id, todoDto, userId);

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
        var success = await _deleteTodoUseCase.ExecuteAsync(id, userId);
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
        var todo = await _toggleTodoUseCase.ExecuteAsync(id, userId);
        if (todo == null)
        {
            return NotFound(
                ApiResponseHelper.Error(404, "Todo not found", $"No Todo Item with ID {id}")
            );
        }

        return Ok(ApiResponseHelper.Success(todo, "Todo status updated successfully."));
    }
}
