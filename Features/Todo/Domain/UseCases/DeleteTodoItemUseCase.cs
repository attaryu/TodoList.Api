using TodoList.Api.Core.Domain.Repositories;
using TodoList.Api.Features.Todo.Domain.Repositories;

namespace TodoList.Api.Features.Todo.Domain.UseCases;

public class DeleteTodoItemUseCase(ITodoRepository todoRepository, IUnitOfWork unitOfWork)
{
    private readonly ITodoRepository _todoRepository = todoRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<bool> ExecuteAsync(int id)
    {
        var todoItem = await _todoRepository.GetByIdAsync(id);
        if (todoItem == null)
        {
            return false;
        }

        _todoRepository.Delete(todoItem);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
