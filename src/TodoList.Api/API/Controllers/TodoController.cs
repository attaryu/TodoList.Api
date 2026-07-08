using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sindika.AspNet.Request;
using Sindika.AspNet.Response;
using TodoList.Api.API.Controllers.Base;
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
    public async Task<IActionResult> GetTodos()
    {
        var userId = GetCurrentUserId();
        var todos = await _todoService.GetAllByUserIdAsync(userId);
        return Ok(ResponseHelper.Success<object>(todos, "Todos retrieved successfully."));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTodo(Guid id)
    {
        var userId = GetCurrentUserId();
        var todo = await _todoService.GetByIdAsync(id, userId);

        if (todo == null)
        {
            return NotFound(
                ResponseHelper.Error<object, object>(
                    null,
                    "ERR-TODO-404",
                    $"No Todo Item with ID {id}"
                )
            );
        }

        return Ok(ResponseHelper.Success<object>(todo, "Todo retrieved successfully."));
    }

    [HttpPost]
    public async Task<IActionResult> CreateTodo([FromBody] BaseRequest<CreateTodoDto> request)
    {
        var userId = GetCurrentUserId();
        var createdTodo = await _todoService.CreateAsync(request.Data, userId);
        return Ok(
            ResponseHelper.Success<CreateTodoDto>(
                createdTodo,
                "Todo created successfully.",
                null,
                request
            )
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodo(
        Guid id,
        [FromBody] BaseRequest<UpdateTodoDto> request
    )
    {
        var userId = GetCurrentUserId();
        var updatedTodo = await _todoService.UpdateAsync(id, request.Data, userId);

        if (updatedTodo == null)
        {
            return NotFound(
                ResponseHelper.Error<object, object>(
                    null,
                    "ERR-TODO-404",
                    $"No Todo Item with ID {id}"
                )
            );
        }

        return Ok(
            ResponseHelper.Success<UpdateTodoDto>(
                updatedTodo,
                "Todo updated successfully.",
                null,
                request
            )
        );
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodo(Guid id)
    {
        var userId = GetCurrentUserId();
        var success = await _todoService.DeleteAsync(id, userId);
        if (!success)
        {
            return NotFound(
                ResponseHelper.Error<object, object>(
                    null,
                    "ERR-TODO-404",
                    $"No Todo Item with ID {id}"
                )
            );
        }

        return Ok(ResponseHelper.Success<object>(null, "Todo deleted successfully."));
    }

    [HttpPatch("{id}/toggle")]
    public async Task<IActionResult> ToggleTodo(Guid id)
    {
        var userId = GetCurrentUserId();
        var todo = await _todoService.ToggleAsync(id, userId);
        if (todo == null)
        {
            return NotFound(
                ResponseHelper.Error<object, object>(
                    null,
                    "ERR-TODO-404",
                    $"No Todo Item with ID {id}"
                )
            );
        }

        return Ok(ResponseHelper.Success<object>(todo, "Todo status updated successfully."));
    }
}
