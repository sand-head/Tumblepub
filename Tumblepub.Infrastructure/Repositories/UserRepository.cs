using Marten;
using Microsoft.Extensions.Logging;
using Tumblepub.Application.Models;

namespace Tumblepub.Infrastructure.Repositories;

internal class UserRepository : MartenRepository<User>
{
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(ILogger<UserRepository> logger, IDocumentSession session)
        : base(session)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override Task<long> DeleteAsync(User aggregate, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
