using TodoList.Api.Domain.Entities;

namespace TodoList.Api.Application.Interfaces.Repositories;

public interface IApiKeyRepository : IBaseRepository<ApiKey>
{
    Task<IEnumerable<ApiKey>> GetAllByUserIdAsync(Guid userId);
    Task<ApiKey?> GetByIdAndUserIdAsync(Guid id, Guid userId);
    Task<int> CountActiveByUserIdAsync(Guid userId);
}
