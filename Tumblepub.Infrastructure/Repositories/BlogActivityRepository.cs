using System.Linq.Expressions;
using Marten;
using Microsoft.Extensions.Logging;
using Tumblepub.Application.Interfaces;
using Tumblepub.Application.Models;

namespace Tumblepub.Infrastructure.Repositories;

internal class BlogActivityRepository : IBlogActivityRepository
{
    private readonly ILogger<BlogActivityRepository> _logger;
    private readonly IDocumentSession _session;

    public BlogActivityRepository(ILogger<BlogActivityRepository> logger, IDocumentSession session)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    public async Task<BlogActivity?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return await _session.LoadAsync<BlogActivity>(id, token);
    }

    public async Task<IEnumerable<BlogActivity>> GetByBlogIdAsync(Guid blogId, int page = 0, CancellationToken token = default)
    {
        return await _session.Query<BlogActivity>()
            .OrderByDescending(a => a.PublishedAt)
            .Where(a => a.BlogId == blogId)
            .Skip(page * 25)
            .Take(25)
            .ToListAsync(token);
    }

    public async Task<int> CountAsync(Expression<Func<BlogActivity, bool>>? condition = null, CancellationToken token = default)
    {
        var query = _session.Query<BlogActivity>();

        if (condition != null)
        {
            return await _session.Query<BlogActivity>()
                .CountAsync(condition, token);
        }

        return await _session.Query<BlogActivity>()
            .CountAsync(token);
    }
}
