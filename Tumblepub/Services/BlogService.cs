using Marten;
using Tumblepub.Database.Events;
using Tumblepub.Database.Projections;
using Tumblepub.Infrastructure;
using Tumblepub.Themes;

namespace Tumblepub.Services;

public interface IBlogService
{
    Task<Blog> CreateAsync(Guid userId, string name);
    Task<IResult> RenderAsync(string name);
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

        _session.Events.StartStream<Blog>(blogCreated.UserId, blogCreated);
        await _session.SaveChangesAsync();
        _logger.LogDebug("Created new blog {Id} with name {Name}", blogCreated.BlogId, name);

        var blog = await _session.Events.AggregateStreamAsync<Blog>(blogCreated.UserId);
        return blog!;
    }

    public async Task<IResult> RenderAsync(string name)
    {
        // todo: get blog from projection
        var data = new
        {
            Title = name,
            Avatar = $"/api/assets/avatar/{name}",
            Posts = new List<object>()
        };

        // todo: add ThemeService to allow for custom themes
        var page = DefaultTheme.Template.Value(data);
        return Results.Content(page, "text/html");
    }
}
