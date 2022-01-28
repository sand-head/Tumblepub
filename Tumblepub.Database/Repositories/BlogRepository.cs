using Marten;
using Microsoft.Extensions.Logging;
using Tumblepub.Database.Events;
using Tumblepub.Database.Infrastructure;
using Tumblepub.Database.Models;

namespace Tumblepub.Database.Repositories;

public interface IBlogRepository
{
    Task<Blog> CreateAsync(Guid userId, string name);
    Task<Blog?> GetByNameAsync(string name, string? domain, CancellationToken cancellationToken = default);
    Task<Blog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Blog>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
}

public class BlogRepository : IBlogRepository
{
    private readonly ILogger<UserRepository> _logger;
    private readonly IDocumentSession _session;

    public BlogRepository(ILogger<UserRepository> logger, IDocumentSession session)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    public async Task<Blog> CreateAsync(Guid userId, string name)
    {
        // generate public/private keys and create initial BlogCreated event
        var (publicKey, privateKey) = CryptoUtils.CreateKeyPair();
        var blogCreated = new BlogCreated(Guid.NewGuid(), userId, name, publicKey, privateKey, DateTimeOffset.UtcNow);

        _session.Events.StartStream<Blog>(blogCreated.BlogId, blogCreated);
        await _session.SaveChangesAsync();
        _logger.LogInformation("Created new blog {Id} with name {Name}", blogCreated.BlogId, blogCreated.BlogName);

        var blog = await _session.LoadAsync<Blog>(blogCreated.BlogId);
        return blog!;
    }

    public async Task<Blog?> GetByNameAsync(string name, string? domain, CancellationToken cancellationToken = default)
    {
        // todo: also filter by domain
        return await _session.Query<Blog>()
            .Where(b => b.Name == name)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Blog?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _session.LoadAsync<Blog>(id, cancellationToken);
    }

    public async Task<IEnumerable<Blog>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
    {
        return await _session.LoadManyAsync<Blog>(cancellationToken, ids);
    }
}
