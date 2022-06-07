using Tumblepub.Application.Models;

namespace Tumblepub.Application.Events;

/// <summary>
/// A <see cref="Projections.User"/> has been created.
/// </summary>
public record UserCreated(
    UserId UserId,
    string Email,
    string PasswordHash,
    DateTimeOffset At);

/// <summary>
/// A <see cref="Projections.User"/> has been deleted.
/// </summary>
public record UserDeleted(
    UserId UserId,
    DateTimeOffset At);