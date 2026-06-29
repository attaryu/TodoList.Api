using Microsoft.AspNetCore.Mvc;
using TodoList.Api.Features.Todo.Domain.Entities;
using TodoList.Api.Features.Todo.Domain.UseCases;

namespace TodoList.Api.Features.Todo.Presentation.Controller;

[ApiController]
[Route("api/[controller]")]
public class TodoController(
    GetTodoItemUseCase getTodoItemUseCase,
    GetTodoItemsUseCase getTodoItemsUseCase,
    CreateTodoItemUseCase CreateTodoItemUseCase,
    UpdateTodoItemUseCase updateTodoItemUseCase,
    DeleteTodoItemUseCase deleteTodoItemUseCase,
    ToggleTodoItemUseCase toggleTodoItemUseCase) : ControllerBase
{
    private readonly GetTodoItemUseCase _getTodoItemUseCase = getTodoItemUseCase;
    private readonly GetTodoItemsUseCase _getTodoItemsUseCase = getTodoItemsUseCase;
    private readonly CreateTodoItemUseCase _CreateTodoItemUseCase = CreateTodoItemUseCase;
    private readonly UpdateTodoItemUseCase _updateTodoItemUseCase = updateTodoItemUseCase;
    private readonly DeleteTodoItemUseCase _deleteTodoItemUseCase = deleteTodoItemUseCase;
    private readonly ToggleTodoItemUseCase _toggleTodoItemUseCase = toggleTodoItemUseCase;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodos()
    {
        var todos = await _getTodoItemsUseCase.ExecuteAsync();
        return Ok(todos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItem>> GetTodo(int id)
    {
        var todo = await _getTodoItemUseCase.ExecuteAsync(id);

        if (todo == null)
        {
            return NotFound(new { message = "Todo not found" });
        }

        return todo;
    }

    [HttpPost]
    public async Task<ActionResult<TodoItem>> CreateTodo(TodoItem todoItem)
    {
        try
        {
            var createdTodo = await _CreateTodoItemUseCase.ExecuteAsync(todoItem);
            return CreatedAtAction(nameof(GetTodo), new { id = createdTodo.Id }, createdTodo);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut]
    public async Task<ActionResult<TodoItem>> UpdateTodo(int id, TodoItem todoItem)
    {
        if (id != todoItem.Id)
        {
            return BadRequest(new { message = "ID mismatch" });
        }

        var updatedTodo = await _updateTodoItemUseCase.ExecuteAsync(id, todoItem);
        if (updatedTodo == null)
        {
            return NotFound(new { message = "Todo not found" });
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodo(int id)
    {
        var success = await _deleteTodoItemUseCase.ExecuteAsync(id);
        if (!success)
        {
            return NotFound(new { message = "Todo not found" });
        }

        return NoContent();
    }

    [HttpPatch("{id}/toggle")]
    public async Task<IActionResult> ToggleTodo(int id)
    {
        var todo = await _toggleTodoItemUseCase.ExecuteAsync(id);
        if (todo == null)
        {
            return NotFound(new { message = "Todo not found" });
        }

        return Ok(todo);
    }
}
