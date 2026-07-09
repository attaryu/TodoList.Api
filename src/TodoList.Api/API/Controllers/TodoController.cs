using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sindika.AspNet.Response;
using TodoList.Api.API.Controllers.Base;
using TodoList.Api.Application.DTOs.Common;
using TodoList.Api.Application.DTOs.Todo.Inputs;
using TodoList.Api.Application.DTOs.Todo.Outputs;
using TodoList.Api.Application.Interfaces.Services;

namespace TodoList.Api.API.Controllers;

[Authorize]
[Route("api/[controller]")]
public class TodoController(ITodoService todoService) : BaseApiController
{
    private readonly ITodoService _todoService = todoService;

    [HttpGet]
    public async Task<IActionResult> GetTodos([FromQuery] int page = 1, [FromQuery] int limit = 10)
    {
        if (page < 1)
            page = 1;
        if (limit < 1)
            limit = 10;
        if (limit > 100)
            limit = 100;

        var userId = GetCurrentUserId();
        var pagedTodos = await _todoService.GetPagedByUserIdAsync(userId, page, limit);

        return Ok(
            ResponseHelper.Success<PagedResultDto<TodoResultDto>>(
                pagedTodos,
                "Todos retrieved successfully."
            )
        );
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTodo(Guid id)
    {
        var userId = GetCurrentUserId();
        var todo = await _todoService.GetByIdAsync(id, userId);

        return Ok(ResponseHelper.Success<object>(todo, "Todo retrieved successfully."));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTodo([FromBody] CreateTodoDto request)
    {
        var userId = GetCurrentUserId();
        var createdTodo = await _todoService.CreateAsync(request, userId);

        return Ok(ResponseHelper.Success<CreateTodoDto>(createdTodo, "Todo created successfully."));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodo(Guid id, [FromBody] UpdateTodoDto request)
    {
        var userId = GetCurrentUserId();
        var updatedTodo = await _todoService.UpdateAsync(id, request, userId);

        return Ok(ResponseHelper.Success<UpdateTodoDto>(updatedTodo, "Todo updated successfully."));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodo(Guid id)
    {
        var userId = GetCurrentUserId();
        var success = await _todoService.DeleteAsync(id, userId);

        return Ok(ResponseHelper.Success<object>(null, "Todo deleted successfully."));
    }

    [HttpPatch("{id}/toggle")]
    public async Task<IActionResult> ToggleTodo(Guid id)
    {
        var userId = GetCurrentUserId();
        var todo = await _todoService.ToggleAsync(id, userId);

        return Ok(ResponseHelper.Success<object>(todo, "Todo status updated successfully."));
    }
}
