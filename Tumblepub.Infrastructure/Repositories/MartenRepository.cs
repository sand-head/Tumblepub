using System.ComponentModel;
using System.Linq.Expressions;
using Marten;
using Tumblepub.Application.Interfaces;
using Tumblepub.Application.Models;

namespace Tumblepub.Infrastructure.Repositories;

internal abstract class MartenQueryableRepository<TAggregate, TId> : IQueryableRepository<TAggregate, TId>, IRepository<TAggregate, TId>
    where TAggregate : class, ISelfAggregate<TId>
    where TId : struct
{
    protected readonly IDocumentSession Session;
    
    public MartenQueryableRepository(IDocumentSession session)
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
        
        Session.Events.StartStream<TAggregate>(GetGuid(aggregate.Id), events);
        
        return events.Length;
    }

    /// <inheritdoc />
    public virtual async Task<long> UpdateAsync(TAggregate aggregate, CancellationToken token = default)
    {
        var events = aggregate.DequeueUncommittedEvents();
        
        Session.Events.Append(GetGuid(aggregate.Id), aggregate.Version, events);
        
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

    public async Task<TAggregate?> GetByIdAsync(TId id, CancellationToken token = default)
    {
        return await Session.LoadAsync<TAggregate>(GetGuid(id), token);
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
    
    private Guid GetGuid(TId id)
    {
        return (Guid)TypeDescriptor.GetConverter(typeof(TId)).ConvertTo(id, typeof(Guid))!;
    }
}