using Tumblepub.Application.Interfaces;
using Tumblepub.Application.Models;

namespace Tumblepub.Application.Extensions;

public static class BlogRepositoryExtensions
{
    public static async Task<Models.Blog?> GetByNameAsync(this IReadOnlyRepository<Models.Blog, Guid> blogRepository, string name, string? domain, CancellationToken cancellationToken = default)
    {
        // todo: also filter by domain
        return await blogRepository.FirstOrDefaultAsync(b => b.Name == name, cancellationToken);
    }

    public static async Task<IEnumerable<Models.Blog>> GetByIdsAsync(this IQueryableRepository<Models.Blog, Guid> blogQueryableRepository, IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return blogQueryableRepository.Query()
            .Where(b => ids.Contains(b.Id))
            .ToList();
    }

    public static async Task<IEnumerable<Models.Blog>> GetByUserIdAsync(this IQueryableRepository<Models.Blog, Guid> blogQueryableRepository, Guid userId, CancellationToken cancellationToken = default)
    {
        return blogQueryableRepository.Query()
            .Where(b => b.UserId.HasValue && b.UserId.Value == userId)
            .ToList();
    }
}