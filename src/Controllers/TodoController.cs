using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.Api.src.Data;
using TodoList.Api.src.Models;

namespace TodoList.Api.src.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoController(AppDbContext appDbContext) : ControllerBase
{
    private readonly AppDbContext _appDbContext = appDbContext;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodos()
    {
        return await _appDbContext.TodoItems.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoItem>> GetTodo(int id)
    {
        var todo = await _appDbContext.TodoItems.FindAsync(id);

        if (todo == null)
        {
            return NotFound(new { message = "Todo not found" });
        }

        return todo;
    }

    [HttpPost]
    public async Task<ActionResult<TodoItem>> CreateTodo(TodoItem todoItem)
    {
        if (string.IsNullOrWhiteSpace(todoItem.Title))
        {
            return BadRequest(new { message = "Title is required" });
        }

        _appDbContext.TodoItems.Add(todoItem);
        await _appDbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetTodo), new { id = todoItem.Id }, todoItem);
    }

    [HttpPut]
    public async Task<ActionResult<TodoItem>> UpdateTodo(int id, TodoItem todoItem)
    {
        if (id != todoItem.Id)
        {
            return BadRequest(new { message = "ID mismatch" });
        }

        var existingTodoItem = await _appDbContext.TodoItems.FindAsync(id);
        if (existingTodoItem == null)
        {
            return NotFound(new { message = "Todo not found" });
        }

        existingTodoItem.Title = todoItem.Title;
        existingTodoItem.Description = todoItem.Description;
        existingTodoItem.IsCompleted = todoItem.IsCompleted;
        existingTodoItem.CompletedAt = todoItem.IsCompleted ? DateTime.UtcNow : null;

        try
        {
            await _appDbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TodoItemExists(id))
            {
                return NotFound();
            }

            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodo(int id)
    {
        var todoItem = await _appDbContext.TodoItems.FindAsync(id);
        if (todoItem == null)
        {
            return NotFound(new { message = "Todo not found" });
        }

        _appDbContext.TodoItems.Remove(todoItem);
        await _appDbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpPatch("{id}/toggle")]
    public async Task<IActionResult> ToggleTodo(int id)
    {
        var todo = await _appDbContext.TodoItems.FindAsync(id);
        if (todo == null)
        {
            return NotFound(new { message = "Todo not found" });
        }

        todo.IsCompleted = !todo.IsCompleted;
        todo.CompletedAt = todo.IsCompleted ? DateTime.UtcNow : null;

        await _appDbContext.SaveChangesAsync();

        return Ok(todo);
    }

    // helper method

    private bool TodoItemExists(int id)
    {
        return _appDbContext.TodoItems.Any(e => e.Id == id);
    }
}
