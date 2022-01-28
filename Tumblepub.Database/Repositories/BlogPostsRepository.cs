using Marten;
using Microsoft.Extensions.Logging;
using Tumblepub.Database.Models;

namespace Tumblepub.Database.Repositories;

public interface IBlogPostsRepository
{
    Task<BlogPosts?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

public class BlogPostsRepository : IBlogPostsRepository
{
    private readonly ILogger<UserRepository> _logger;
    private readonly IDocumentSession _session;

    public BlogPostsRepository(ILogger<UserRepository> logger, IDocumentSession session)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    public async Task<BlogPosts?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _session.LoadAsync<BlogPosts>(id, cancellationToken);
    }
}
