using System.Text.Json;

namespace Tumblepub.Database.Events;

/// <summary>
/// A <see cref="Projections.Blog"/> has been created on this instance.
/// </summary>
public record BlogCreated(
    Guid BlogId,
    Guid UserId,
    string BlogName,
    string PublicKey,
    string PrivateKey,
    DateTimeOffset At);

/// <summary>
/// A <see cref="Projections.Blog"/> has been discovered on another instance.
/// </summary>
public record BlogDiscovered(
    Guid BlogId,
    string BlogName,
    string PublicKey,
    DateTimeOffset At);

/// <summary>
/// A <see cref="Projections.Blog"/>'s metadata has been updated.
/// </summary>
public record BlogMetadataUpdated(
    Guid BlogId,
    string? Title,
    string? Description,
    JsonDocument? Metadata,
    DateTimeOffset At);

/// <summary>
/// A <see cref="Projections.Blog"/> has been deleted.
/// </summary>
public record BlogDeleted(
    Guid BlogId,
    DateTimeOffset At);
