using Tumblepub.Application.Models;

namespace Tumblepub.Application.Interfaces;

public interface IBlogRepository
{
    Task<Blog> CreateAsync(Guid userId, string name, CancellationToken cancellationToken = default);
    Task<Blog?> GetByNameAsync(string name, string? domain, CancellationToken cancellationToken = default);
    Task<Blog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Blog>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<IEnumerable<Blog>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}