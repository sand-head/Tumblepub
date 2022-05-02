using Tumblepub.Application.Interfaces;

namespace Tumblepub.Application.Extensions;

public static class BlogRepositoryExtensions
{
    public static async Task<Models.Blog?> GetByNameAsync(this IRepository<Models.Blog> blogRepository, string name, string? domain, CancellationToken cancellationToken = default)
    {
        // todo: also filter by domain
        return await blogRepository.Query()
            .Where(b => b.Name == name)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public static async Task<IEnumerable<Models.Blog>> GetByIdsAsync(this IRepository<Models.Blog> blogRepository, IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await blogRepository.Query()
            .Where(b => ids.Contains(b.Id))
            .ToListAsync(cancellationToken);
    }

    public static async Task<IEnumerable<Models.Blog>> GetByUserIdAsync(this IRepository<Models.Blog> blogRepository, Guid userId, CancellationToken cancellationToken = default)
    {
        return await blogRepository.Query()
            .Where(b => b.UserId == userId)
            .ToListAsync(cancellationToken);
    }
}