using Marten;
using Microsoft.Extensions.Logging;
using Tumblepub.Application.Aggregates;

namespace Tumblepub.Infrastructure.Repositories;

internal class PostRepository : MartenRepository<Post>
{
    private readonly ILogger<PostRepository> _logger;

    public PostRepository(ILogger<PostRepository> logger, IDocumentSession session)
        : base(session)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override Task<long> DeleteAsync(Post aggregate, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
