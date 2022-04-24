namespace Tumblepub.Application.Events;

/// <summary>
/// A <see cref="Projections.User"/> has been created.
/// </summary>
public record UserCreated(
    Guid UserId,
    string Email,
    string PasswordHash,
    DateTimeOffset At);

/// <summary>
/// A <see cref="Projections.User"/> has been deleted.
/// </summary>
public record UserDeleted(
    Guid UserId,
    DateTimeOffset At);