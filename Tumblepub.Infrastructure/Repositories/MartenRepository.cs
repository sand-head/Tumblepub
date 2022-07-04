using System.Linq.Expressions;
using Marten;
using Tumblepub.Application.Aggregates;
using Tumblepub.Application.Interfaces;

namespace Tumblepub.Infrastructure.Repositories;

internal abstract class MartenRepository<TAggregate> : IQueryableRepository<TAggregate, Guid>, IRepository<TAggregate, Guid>
    where TAggregate : class, ISelfAggregate<Guid>
{
    protected readonly IDocumentSession Session;

    protected MartenRepository(IDocumentSession session)
    {
        Session = session;
    }

    /// <inheritdoc />
    public IQueryable<TAggregate> Query()
    {
        return Session.Query<TAggregate>();
    }

    /// <inheritdoc />
    public virtual async Task<long> CreateAsync(TAggregate aggregate, CancellationToken token = default)
    {
        var events = aggregate.DequeueUncommittedEvents();
        
        Session.Events.StartStream<TAggregate>(aggregate.Id, events);
        
        return events.Length;
    }

    /// <inheritdoc />
    public virtual async Task<long> UpdateAsync(TAggregate aggregate, CancellationToken token = default)
    {
        var events = aggregate.DequeueUncommittedEvents();
        
        Session.Events.Append(aggregate.Id, aggregate.Version, events);
        
        return aggregate.Version;
    }

    public abstract Task<long> DeleteAsync(TAggregate aggregate, CancellationToken token = default);

    public async Task<IEnumerable<TAggregate>> GetAllAsync(Expression<Func<TAggregate, bool>>? whereCondition, CancellationToken token = default)
    {
        var queryable = Session.Query<TAggregate>();
        if (whereCondition != null)
        {
            return await queryable.Where(whereCondition).ToListAsync(token);
        }

        return await queryable.ToListAsync(token);
    }

    public async Task<TAggregate?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return await Session.LoadAsync<TAggregate>(id, token);
    }

    public async Task<TAggregate?> FirstOrDefaultAsync(Expression<Func<TAggregate, bool>>? whereCondition, CancellationToken token = default)
    {
        var queryable = Session.Query<TAggregate>();
        if (whereCondition != null)
        {
            return await queryable.Where(whereCondition).FirstOrDefaultAsync(token);
        }

        return await queryable.FirstOrDefaultAsync(token);
    }

    public async Task SaveChangesAsync(CancellationToken token = default)
    {
        await Session.SaveChangesAsync(token);
    }

    public void Dispose()
    {
        Session.Dispose();
    }
}