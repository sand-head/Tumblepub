namespace Tumblepub.Application.Models;

public interface IAggregate<out TId>
{
    TId Id { get; }
    int Version { get; }
}

public interface ISelfAggregate<out TId> : IAggregate<TId>
{
    object[] DequeueUncommittedEvents();
}

public class ReadOnlyAggregate<TId> : IAggregate<TId>
    where TId : notnull
{
    public TId Id { get; protected set; } = default!;
    public int Version { get; protected set; }
}

public class Aggregate<TId> : ReadOnlyAggregate<TId>, ISelfAggregate<TId>
    where TId : notnull
{
    [NonSerialized]
    private readonly Queue<object> _uncommittedEvents = new();
    
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