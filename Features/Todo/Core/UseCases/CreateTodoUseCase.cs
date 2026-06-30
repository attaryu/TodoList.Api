using TodoList.Api.Shared.Domain.Repositories;
using TodoList.Api.Features.Todo.Core.Entities;
using TodoList.Api.Features.Todo.Core.Repositories;

namespace TodoList.Api.Features.Todo.Core.UseCases;

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
