using Microsoft.EntityFrameworkCore.Storage;
using TodoList.Api.Core.Domain.Repositories;
using TodoList.Api.Core.Infrastructure.Persistent;

namespace TodoList.Api.Core.Infrastructure.Repositories;

public class UnitOfWorkImpl(AppDbContext appDbContext) : IUnitOfWork
{
    private readonly AppDbContext _appDbContext = appDbContext;
    private IDbContextTransaction? _transaction;


    public async Task<int> SaveChangesAsync()
    {
        return await _appDbContext.SaveChangesAsync();
    }
    public async Task BeginTransactionAsync()
    {
        _transaction = await _appDbContext.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        GC.SuppressFinalize(this);
    }
}