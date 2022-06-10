using Marten;
using Microsoft.Extensions.Logging;
using Tumblepub.Application.Events;
using Tumblepub.Application.Interfaces;
using Tumblepub.Application.Models;
using Tumblepub.Infrastructure.Infrastructure;

namespace Tumblepub.Infrastructure.Repositories;

internal class BlogRepository : MartenRepository<Blog>
{
    private readonly ILogger<BlogRepository> _logger;

    public BlogRepository(ILogger<BlogRepository> logger, IDocumentSession session)
        : base(session)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override Task<long> DeleteAsync(Blog aggregate, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
