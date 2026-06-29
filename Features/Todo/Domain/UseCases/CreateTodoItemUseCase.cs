using TodoList.Api.Core.Domain.Repositories;
using TodoList.Api.Features.Todo.Domain.Entities;
using TodoList.Api.Features.Todo.Domain.Repositories;

namespace TodoList.Api.Features.Todo.Domain.UseCases;

public class CreateTodoItemUseCase(ITodoRepository todoRepository, IUnitOfWork unitOfWork)
{
    private readonly ITodoRepository _todoRepository = todoRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<TodoItem> ExecuteAsync(TodoItem todoItem)
    {
        if (string.IsNullOrWhiteSpace(todoItem.Title))
        {
            throw new ArgumentException("Title is required");
        }

        await _todoRepository.AddAsync(todoItem);
        await _unitOfWork.SaveChangesAsync();

        return todoItem;
    }
}
