using TodoList.Api.Core.Domain.Repositories;
using TodoList.Api.Features.Todo.Domain.Repositories;

namespace TodoList.Api.Features.Todo.Domain.UseCases;

public class DeleteTodoUseCase(ITodoRepository todoRepository, IUnitOfWork unitOfWork)
{
    private readonly ITodoRepository _todoRepository = todoRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<bool> ExecuteAsync(int id)
    {
        var todo = await _todoRepository.GetByIdAsync(id);
        if (todo == null)
        {
            return false;
        }

        _todoRepository.Delete(todo);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
