using Microsoft.EntityFrameworkCore;
using TodoList.Api.Features.Todo.Core.Entities;
using TodoList.Api.Features.Todo.Core.Repositories;
using TodoList.Api.Shared.Infrastructure.Persistent;
using TodoList.Api.Shared.Infrastructure.Repositories;

namespace TodoList.Api.Features.Todo.Infrastructure.Persistents.Repositories;

public class TodoRepositoryImpl(AppDbContext appDbContext)
    : BaseRepositoryImpl<TodoItem>(appDbContext),
        ITodoRepository
{
    public async Task<IEnumerable<TodoItem>> GetAllByUserIdAsync(int userId)
    {
        return await _dbSet.Where(t => t.UserId == userId).ToListAsync();
    }

    public async Task<TodoItem?> GetByIdAndUserIdAsync(int id, int userId)
    {
        return await _dbSet.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
    }
}
