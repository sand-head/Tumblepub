namespace Tumblepub.EventSourcing.Events;

public record BlogCreated(Guid Id, DateTimeOffset At);
public record BlogDiscovered(Guid Id, DateTimeOffset At);
public record BlogDeleted(Guid Id, DateTimeOffset At);
