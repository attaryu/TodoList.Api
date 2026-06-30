using TodoList.Api.Features.Todo.Domain.Entities;
using TodoList.Api.Features.Todo.Domain.Repositories;

namespace TodoList.Api.Features.Todo.Domain.UseCases;

public class GetTodoUseCase(ITodoRepository todoRepository)
{
    private readonly ITodoRepository _todoRepository = todoRepository;

    public async Task<TodoItem?> ExecuteAsync(int id)
    {
        return await _todoRepository.GetByIdAsync(id);
    }
}
