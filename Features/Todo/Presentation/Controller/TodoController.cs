using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TodoList.Api.Features.Todo.Core.DTOs;
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

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TodoDto>>> GetTodos()
    {
        var todos = await _getTodosUseCase.ExecuteAsync();
        var todoDtos = _mapper.Map<IEnumerable<TodoDto>>(todos);
        return Ok(todoDtos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TodoDto>> GetTodo(int id)
    {
        var todo = await _getTodoUseCase.ExecuteAsync(id);

        if (todo == null)
        {
            return NotFound(new { message = "Todo not found" });
        }

        var todoDto = _mapper.Map<TodoDto>(todo);
        return Ok(todoDto);
    }

    [HttpPost]
    public async Task<ActionResult<TodoDto>> CreateTodo(CreateTodoDto todoDto)
    {
        try
        {
            var todoEntity = _mapper.Map<TodoItem>(todoDto);
            var createdTodo = await _CreateTodoUseCase.ExecuteAsync(todoEntity);
            var createdTodoDto = _mapper.Map<TodoDto>(createdTodo);
            return CreatedAtAction(nameof(GetTodo), new { id = createdTodoDto.Id }, createdTodoDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<TodoDto>> UpdateTodo(int id, UpdateTodoDto todoDto)
    {
        var todoEntity = _mapper.Map<TodoItem>(todoDto);
        var updatedTodo = await _updateTodoUseCase.ExecuteAsync(id, todoEntity);

        if (updatedTodo == null)
        {
            return NotFound(new { message = "Todo not found" });
        }

        var updatedTodoDto = _mapper.Map<TodoDto>(updatedTodo);
        return Ok(updatedTodoDto);
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
    public async Task<ActionResult<TodoDto>> ToggleTodo(int id)
    {
        var todo = await _toggleTodoUseCase.ExecuteAsync(id);
        if (todo == null)
        {
            return NotFound(new { message = "Todo not found" });
        }

        var todoDto = _mapper.Map<TodoDto>(todo);
        return Ok(todoDto);
    }
}
