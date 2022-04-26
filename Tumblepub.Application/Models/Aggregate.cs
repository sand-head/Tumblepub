namespace Tumblepub.Application.Models;

public interface IAggregate<out TId>
{
    TId Id { get; }
    int Version { get; }
    
    object[] DequeueUncommittedEvents();
}

public interface IAggregate : IAggregate<Guid>
{
}

public class Aggregate<TId> : IAggregate<TId>
    where TId : notnull
{
    [NonSerialized]
    private readonly Queue<object> _uncommittedEvents = new();
    
    public TId Id { get; protected set; } = default!;
    public int Version { get; protected set; }
    
    public object[] DequeueUncommittedEvents()
    {
        var events = _uncommittedEvents.ToArray();
        _uncommittedEvents.Clear();
        return events;
    }
    
    protected void Enqueue(object @event)
    {
        _uncommittedEvents.Enqueue(@event);
    }
}

public class Aggregate : Aggregate<Guid>, IAggregate
{
}