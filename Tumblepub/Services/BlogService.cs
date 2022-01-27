using Marten;
using Tumblepub.Events;
using Tumblepub.Infrastructure;
using Tumblepub.Projections;

namespace Tumblepub.Services;

public interface IBlogService
{
    Task<Blog> CreateAsync(Guid userId, string name);
    Task<Blog?> GetByNameAsync(string name, string? domain, CancellationToken cancellationToken = default);
}

public class BlogService : IBlogService
{
    private readonly ILogger<UserService> _logger;
    private readonly IDocumentSession _session;

    public BlogService(ILogger<UserService> logger, IDocumentSession session)
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
            .Where(b => b.BlogName == name)
            .FirstOrDefaultAsync(cancellationToken);
    }
}
