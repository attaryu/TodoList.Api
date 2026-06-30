using TodoList.Api.Core.Domain.Repositories;
using TodoList.Api.Features.Todo.Domain.Entities;
using TodoList.Api.Features.Todo.Domain.Repositories;

namespace TodoList.Api.Features.Todo.Domain.UseCases;

public class CreateTodoUseCase(ITodoRepository todoRepository, IUnitOfWork unitOfWork)
{
    private readonly ITodoRepository _todoRepository = todoRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<TodoItem> ExecuteAsync(TodoItem todo)
    {
        if (string.IsNullOrWhiteSpace(todo.Title))
        {
            throw new ArgumentException("Title is required");
        }

        await _todoRepository.AddAsync(todo);
        await _unitOfWork.SaveChangesAsync();

        return todo;
    }
}
