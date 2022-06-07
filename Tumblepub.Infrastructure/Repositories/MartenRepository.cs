using System.ComponentModel;
using System.Linq.Expressions;
using Marten;
using Tumblepub.Application.Interfaces;
using Tumblepub.Application.Models;

namespace Tumblepub.Infrastructure.Repositories;

public class MartenRepository<TAggregate, TId> : IRepository<TAggregate, TId>, IRepositoryQueryable<TAggregate, TId>
    where TAggregate : class, IAggregate<TId>
    where TId : struct
{
    private readonly IDocumentSession _session;
    
    public MartenRepository(IDocumentSession session)
    {
        _session = session;
    }

    public async Task<TAggregate?> GetByIdAsync(TId id, CancellationToken token = default)
    {
        return await _session.LoadAsync<TAggregate>(GetGuid(id), token);
    }

    /// <inheritdoc />
    public IQueryable<TAggregate> Query()
    {
        return _session.Query<TAggregate>();
    }

    public Task<IEnumerable<TAggregate>> GetAllAsync(Expression<Func<TAggregate, bool>>? whereCondition, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<TAggregate?> FirstOrDefaultAsync(Expression<Func<TAggregate, bool>>? whereCondition, CancellationToken token = default)
    {
        var queryable = _session.Query<TAggregate>();
        if (whereCondition != null)
        {
            return await queryable.Where(whereCondition).FirstOrDefaultAsync(token);
        }

        return await queryable.FirstOrDefaultAsync(token);
    }

    /// <inheritdoc />
    public async Task<long> CreateAsync(TAggregate aggregate, CancellationToken token = default)
    {
        var events = aggregate.DequeueUncommittedEvents();
        
        _session.Events.StartStream<TAggregate>(GetGuid(aggregate.Id), events);
        
        await _session.SaveChangesAsync(token);
        return events.Length;
    }

    /// <inheritdoc />
    public async Task<long> UpdateAsync(TAggregate aggregate, CancellationToken token = default)
    {
        var events = aggregate.DequeueUncommittedEvents();
        
        _session.Events.Append(GetGuid(aggregate.Id), aggregate.Version, events);
        
        await _session.SaveChangesAsync(token);
        return aggregate.Version;
    }

    public Task<long> DeleteAsync(TAggregate aggregate, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public Task<int> SaveChangesAsync(CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
    
    private Guid GetGuid(TId id)
    {
        return (Guid)TypeDescriptor.GetConverter(typeof(TId)).ConvertTo(id, typeof(Guid))!;
    }
}