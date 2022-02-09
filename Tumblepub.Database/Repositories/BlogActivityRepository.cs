using Marten;
using Microsoft.Extensions.Logging;
using Tumblepub.Database.Models;

namespace Tumblepub.Database.Repositories;

public interface IBlogActivityRepository
{
    Task<BlogActivity?> GetByIdAsync(Guid id, CancellationToken token = default);
    Task<IEnumerable<BlogActivity>> GetByBlogIdAsync(Guid blogId, int page = 0, CancellationToken token = default);
}

public class BlogActivityRepository : IBlogActivityRepository
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
}
