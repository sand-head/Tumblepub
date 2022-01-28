using Marten;
using Microsoft.Extensions.Logging;
using Tumblepub.Database.Models;

namespace Tumblepub.Database.Repositories;

public interface IUserBlogsRepository
{
    Task<UserBlogs?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<UserBlogs>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}

public class UserBlogsRepository : IUserBlogsRepository
{
    private readonly ILogger<UserRepository> _logger;
    private readonly IDocumentSession _session;

    public UserBlogsRepository(ILogger<UserRepository> logger, IDocumentSession session)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    public async Task<UserBlogs?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _session.LoadAsync<UserBlogs>(id, cancellationToken);
    }

    public async Task<IEnumerable<UserBlogs>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await _session.LoadManyAsync<UserBlogs>(cancellationToken, ids);
    }
}
