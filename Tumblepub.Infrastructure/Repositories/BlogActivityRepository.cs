using System.ComponentModel;
using System.Linq.Expressions;
using Marten;
using Microsoft.Extensions.Logging;
using Tumblepub.Application.Interfaces;
using Tumblepub.Application.Models;

namespace Tumblepub.Infrastructure.Repositories;

internal class BlogActivityRepository : IQueryableRepository<BlogActivity, BlogActivityId>, IReadOnlyRepository<BlogActivity, BlogActivityId>
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
        throw new NotImplementedException();
    }

    public Task<IEnumerable<BlogActivity>> GetAllAsync(Expression<Func<BlogActivity, bool>>? whereCondition, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<BlogActivity?> GetByIdAsync(BlogActivityId id, CancellationToken token = default)
    {
        return await _session.LoadAsync<BlogActivity>(GetGuid(id), token);
    }

    public Task<BlogActivity?> FirstOrDefaultAsync(Expression<Func<BlogActivity, bool>>? whereCondition, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        _session.Dispose();
    }
    
    private Guid GetGuid(BlogActivityId id)
    {
        return (Guid)TypeDescriptor.GetConverter(typeof(BlogActivityId)).ConvertTo(id, typeof(Guid))!;
    }
}
