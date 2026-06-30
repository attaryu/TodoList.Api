using Microsoft.AspNetCore.Mvc;
using TodoList.Api.Features.Todo.Core.Entities;
using TodoList.Api.Features.Todo.Core.UseCases;

namespace TodoList.Api.Features.Todo.Presentation.Controller;

[ApiController]
[Route("api/[controller]")]
public class TodoController(
    GetTodoUseCase getTodoUseCase,
    GetTodosUseCase getTodosUseCase,
    CreateTodoUseCase CreateTodoUseCase,
    UpdateTodoUseCase updateTodoUseCase,
    DeleteTodoUseCase deleteTodoUseCase,
    ToggleTodoUseCase toggleTodoUseCase) : ControllerBase
{
    private readonly GetTodoUseCase _getTodoUseCase = getTodoUseCase;
    private readonly GetTodosUseCase _getTodosUseCase = getTodosUseCase;
    private readonly CreateTodoUseCase _CreateTodoUseCase = CreateTodoUseCase;
    private readonly UpdateTodoUseCase _updateTodoUseCase = updateTodoUseCase;
    private readonly DeleteTodoUseCase _deleteTodoUseCase = deleteTodoUseCase;
    private readonly ToggleTodoUseCase _toggleTodoUseCase = toggleTodoUseCase;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodos()
    {
        var todos = await _getTodosUseCase.ExecuteAsync();
        return Ok(todos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItem>> GetTodo(int id)
    {
        var todo = await _getTodoUseCase.ExecuteAsync(id);

        if (todo == null)
        {
            return NotFound(new { message = "Todo not found" });
        }

        return todo;
    }

    [HttpPost]
    public async Task<ActionResult<TodoItem>> CreateTodo(TodoItem Todo)
    {
        try
        {
            var createdTodo = await _CreateTodoUseCase.ExecuteAsync(Todo);
            return CreatedAtAction(nameof(GetTodo), new { id = createdTodo.Id }, createdTodo);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut]
    public async Task<ActionResult<TodoItem>> UpdateTodo(int id, TodoItem Todo)
    {
        if (id != Todo.Id)
        {
            return BadRequest(new { message = "ID mismatch" });
        }

        var updatedTodo = await _updateTodoUseCase.ExecuteAsync(id, Todo);
        if (updatedTodo == null)
        {
            return NotFound(new { message = "Todo not found" });
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodo(int id)
    {
        var success = await _deleteTodoUseCase.ExecuteAsync(id);
        if (!success)
        {
            return NotFound(new { message = "Todo not found" });
        }

        return NoContent();
    }

    [HttpPatch("{id}/toggle")]
    public async Task<IActionResult> ToggleTodo(int id)
    {
        var todo = await _toggleTodoUseCase.ExecuteAsync(id);
        if (todo == null)
        {
            return NotFound(new { message = "Todo not found" });
        }

        return Ok(todo);
    }
}
