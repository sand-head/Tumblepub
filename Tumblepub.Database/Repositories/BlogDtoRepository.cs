using Marten;
using Microsoft.Extensions.Logging;
using Tumblepub.Database.Models;

namespace Tumblepub.Database.Repositories;

public interface IBlogDtoRepository
{
    Task<BlogDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}

public class BlogDtoRepository : IBlogDtoRepository
{
    private readonly ILogger<UserRepository> _logger;
    private readonly IDocumentSession _session;

    public BlogDtoRepository(ILogger<UserRepository> logger, IDocumentSession session)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    public async Task<BlogDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _session.LoadAsync<BlogDto>(id, cancellationToken);
    }
}
