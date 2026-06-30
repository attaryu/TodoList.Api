using TodoList.Api.Shared.Domain.Repositories;
using TodoList.Api.Features.Todo.Core.Repositories;

namespace TodoList.Api.Features.Todo.Core.UseCases;

public class DeleteTodoUseCase(ITodoRepository todoRepository, IUnitOfWork unitOfWork)
{
    private readonly ITodoRepository _todoRepository = todoRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<bool> ExecuteAsync(int id, int userId)
    {
        var todo = await _todoRepository.GetByIdAndUserIdAsync(id, userId);
        if (todo == null)
        {
            return false;
        }

        _todoRepository.Delete(todo);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
