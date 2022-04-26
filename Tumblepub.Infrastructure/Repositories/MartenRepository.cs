using Marten;
using Tumblepub.Application.Interfaces;
using Tumblepub.Application.Models;

namespace Tumblepub.Infrastructure.Repositories;

public class MartenRepository<TAggregate> : IRepository<TAggregate>
    where TAggregate : class, IAggregate
{
    private readonly IDocumentSession _session;
    
    public MartenRepository(IDocumentSession session)
    {
        _session = session;
    }

    /// <inheritdoc />
    public async Task<TAggregate?> GetByIdAsync(Guid id, CancellationToken token = default)
    {
        return await _session.Events.AggregateStreamAsync<TAggregate>(id, token: token);
    }

    /// <inheritdoc />
    public IQueryable<TAggregate> Query()
    {
        return _session.Query<TAggregate>();
    }
    
    /// <inheritdoc />
    public async Task<long> CreateAsync(TAggregate aggregate, CancellationToken token = default)
    {
        var events = aggregate.DequeueUncommittedEvents();
        
        _session.Events.StartStream<TAggregate>(aggregate.Id, events);
        
        await _session.SaveChangesAsync(token);
        return events.Length;
    }

    /// <inheritdoc />
    public async Task<long> UpdateAsync(TAggregate aggregate, CancellationToken token = default)
    {
        var events = aggregate.DequeueUncommittedEvents();
        
        _session.Events.Append(aggregate.Id, aggregate.Version, events);
        
        await _session.SaveChangesAsync(token);
        return aggregate.Version;
    }
}