using Tumblepub.Application.Interfaces;
using Tumblepub.Application.Models;

namespace Tumblepub.Application.Extensions;

public static class BlogRepositoryExtensions
{
    public static async Task<Blog?> GetByNameAsync(this IRepository<Blog> blogRepository, string name, string? domain, CancellationToken cancellationToken = default)
    {
        // todo: also filter by domain
        return await blogRepository.Query()
            .Where(b => b.Name == name)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public static async Task<IEnumerable<Blog>> GetByIdsAsync(this IRepository<Blog> blogRepository, IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await blogRepository.Query()
            .Where(b => ids.Contains(b.Id))
            .ToListAsync(cancellationToken);
    }

    public static async Task<IEnumerable<Blog>> GetByUserIdAsync(this IRepository<Blog> blogRepository, Guid userId, CancellationToken cancellationToken = default)
    {
        return await blogRepository.Query()
            .Where(b => b.UserId == userId)
            .ToListAsync(cancellationToken);
    }
}