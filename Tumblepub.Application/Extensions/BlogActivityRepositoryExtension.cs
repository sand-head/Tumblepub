using System.Linq.Expressions;
using Tumblepub.Application.Interfaces;
using Tumblepub.Application.Models;

namespace Tumblepub.Application.Extensions;

public static class BlogActivityRepositoryExtension
{
    public static async Task<IEnumerable<BlogActivity>> GetByBlogIdAsync(this IQueryableRepository<BlogActivity, BlogActivityId> repository, BlogId blogId, int page = 0, CancellationToken token = default)
    {
        return repository.Query()
            .OrderByDescending(a => a.PublishedAt)
            .Where(a => a.BlogId == blogId)
            .Skip(page * 25)
            .Take(25)
            .ToList();
    }

    public static async Task<int> CountAsync(this IQueryableRepository<BlogActivity, BlogActivityId> repository, Expression<Func<BlogActivity, bool>>? condition = null, CancellationToken token = default)
    {
        return condition != null
            ? repository.Query().Count(condition)
            : repository.Query().Count();
    }
}