using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.Api.Features.Todo.Presentation.DTOs;
using TodoList.Api.Features.Todo.Core.Entities;
using TodoList.Api.Features.Todo.Core.UseCases;
using TodoList.Api.Shared.Presentation.Helpers;
using System.Security.Claims;

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
    ToggleTodoUseCase toggleTodoUseCase,
    IMapper mapper) : ControllerBase
{
    private readonly GetTodoUseCase _getTodoUseCase = getTodoUseCase;
    private readonly GetTodosUseCase _getTodosUseCase = getTodosUseCase;
    private readonly CreateTodoUseCase _CreateTodoUseCase = CreateTodoUseCase;
    private readonly UpdateTodoUseCase _updateTodoUseCase = updateTodoUseCase;
    private readonly DeleteTodoUseCase _deleteTodoUseCase = deleteTodoUseCase;
    private readonly ToggleTodoUseCase _toggleTodoUseCase = toggleTodoUseCase;
    private readonly IMapper _mapper = mapper;

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
    public async Task<ActionResult<ApiResponse<IEnumerable<TodoDto>>>> GetTodos()
    {
        var userId = GetCurrentUserId();
        var todos = await _getTodosUseCase.ExecuteAsync(userId);
        var todoDtos = _mapper.Map<IEnumerable<TodoDto>>(todos);
        return Ok(ApiResponseHelper.Success(todoDtos, "Todos retrieved successfully."));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TodoDto>>> GetTodo(int id)
    {
        var userId = GetCurrentUserId();
        var todo = await _getTodoUseCase.ExecuteAsync(id, userId);

        if (todo == null)
        {
            return NotFound(ApiResponseHelper.Error(404, "Todo not found", $"No Todo Item with ID {id}"));
        }

        var todoDto = _mapper.Map<TodoDto>(todo);
        return Ok(ApiResponseHelper.Success(todoDto, "Todo retrieved successfully."));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<TodoDto>>> CreateTodo(CreateTodoDto todoDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var todoEntity = _mapper.Map<TodoItem>(todoDto);
            var createdTodo = await _CreateTodoUseCase.ExecuteAsync(todoEntity, userId);
            var createdTodoDto = _mapper.Map<TodoDto>(createdTodo);
            var response = ApiResponseHelper.Success(createdTodoDto, "Todo created successfully.");
            return CreatedAtAction(nameof(GetTodo), new { id = createdTodoDto.Id }, response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ApiResponseHelper.Error(400, "Failed to create Todo", ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<TodoDto>>> UpdateTodo(int id, UpdateTodoDto todoDto)
    {
        var userId = GetCurrentUserId();
        var todoEntity = _mapper.Map<TodoItem>(todoDto);
        var updatedTodo = await _updateTodoUseCase.ExecuteAsync(id, todoEntity, userId);

        if (updatedTodo == null)
        {
            return NotFound(ApiResponseHelper.Error(404, "Todo not found", $"No Todo Item with ID {id}"));
        }

        var updatedTodoDto = _mapper.Map<TodoDto>(updatedTodo);
        return Ok(ApiResponseHelper.Success(updatedTodoDto, "Todo updated successfully."));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object?>>> DeleteTodo(int id)
    {
        var userId = GetCurrentUserId();
        var success = await _deleteTodoUseCase.ExecuteAsync(id, userId);
        if (!success)
        {
            return NotFound(ApiResponseHelper.Error(404, "Todo not found", $"No Todo Item with ID {id}"));
        }

        return Ok(ApiResponseHelper.Success<object?>(null, "Todo deleted successfully."));
    }

    [HttpPatch("{id}/toggle")]
    public async Task<ActionResult<ApiResponse<TodoDto>>> ToggleTodo(int id)
    {
        var userId = GetCurrentUserId();
        var todo = await _toggleTodoUseCase.ExecuteAsync(id, userId);
        if (todo == null)
        {
            return NotFound(ApiResponseHelper.Error(404, "Todo not found", $"No Todo Item with ID {id}"));
        }

        var todoDto = _mapper.Map<TodoDto>(todo);
        return Ok(ApiResponseHelper.Success(todoDto, "Todo status updated successfully."));
    }
}
