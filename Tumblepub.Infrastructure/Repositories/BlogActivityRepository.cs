using System.ComponentModel;
using System.Linq.Expressions;
using Marten;
using Microsoft.Extensions.Logging;
using Tumblepub.Application.Aggregates;
using Tumblepub.Application.Interfaces;

namespace Tumblepub.Infrastructure.Repositories;

internal class BlogActivityRepository : IQueryableRepository<BlogActivity, Guid>, IReadOnlyRepository<BlogActivity, Guid>
{
    private readonly ILogger<BlogActivityRepository> _logger;
    private readonly IDocumentSession _session;

    public BlogActivityRepository(ILogger<BlogActivityRepository> logger, IDocumentSession session)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _session = session ?? throw new ArgumentNullException(nameof(session));
    }

    public IQueryable<BlogActivity> Query()
    {
        return _session.Query<BlogActivity>();
    }

    public async Task<IEnumerable<BlogActivity>> GetAllAsync(Expression<Func<BlogActivity, bool>>? whereCondition, CancellationToken token = default)
    {
        var queryable = _session.Query<BlogActivity>();
        if (whereCondition != null)
        {
            return await queryable.Where(whereCondition).ToListAsync(token);
        }

        return await queryable.ToListAsync(token);
    }

    public async Task<BlogActivity?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return await _session.LoadAsync<BlogActivity>(id, token);
    }

    public async Task<BlogActivity?> FirstOrDefaultAsync(Expression<Func<BlogActivity, bool>>? whereCondition, CancellationToken token = default)
    {
        var queryable = _session.Query<BlogActivity>();
        if (whereCondition != null)
        {
            return await queryable.Where(whereCondition).FirstOrDefaultAsync(token);
        }

        return await queryable.FirstOrDefaultAsync(token);
    }

    public void Dispose()
    {
        _session.Dispose();
    }
}
