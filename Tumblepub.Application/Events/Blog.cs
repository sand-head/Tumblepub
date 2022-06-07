using System.Text.Json;
using Tumblepub.Application.Models;

namespace Tumblepub.Application.Events;

/// <summary>
/// A <see cref="Projections.Blog"/> has been created on this instance.
/// </summary>
public record BlogCreated(
    BlogId BlogId,
    UserId UserId,
    string BlogName,
    string PublicKey,
    string PrivateKey,
    DateTimeOffset At);

/// <summary>
/// A <see cref="Projections.Blog"/> has been discovered on another instance.
/// </summary>
public record BlogDiscovered(
    BlogId BlogId,
    string BlogName,
    string PublicKey,
    DateTimeOffset At);

/// <summary>
/// A <see cref="Projections.Blog"/>'s metadata has been updated.
/// </summary>
public record BlogMetadataUpdated(
    BlogId BlogId,
    string? Title,
    string? Description,
    JsonDocument? Metadata,
    DateTimeOffset At);

/// <summary>
/// A <see cref="Projections.Blog"/> has been deleted.
/// </summary>
public record BlogDeleted(
    BlogId BlogId,
    DateTimeOffset At);