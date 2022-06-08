using Marten;
using Microsoft.Extensions.Logging;
using Tumblepub.Application.Models;

namespace Tumblepub.Infrastructure.Repositories;

internal class PostQueryableRepository : MartenQueryableRepository<Post, PostId>
{
    private readonly ILogger<PostQueryableRepository> _logger;

    public PostQueryableRepository(ILogger<PostQueryableRepository> logger, IDocumentSession session)
        : base(session)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override Task<long> DeleteAsync(Post aggregate, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
