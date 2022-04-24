using Tumblepub.Application.Models;

namespace Tumblepub.Application.Events;

public record PostCreated(
    Guid PostId,
    Guid BlogId,
    PostContent Content,
    DateTimeOffset At);

public record PostDiscovered(
    Guid PostId,
    Guid BlogId,
    DateTimeOffset At);

public record PostUpdated(
    Guid PostId,
    DateTimeOffset At);

public record PostLiked(
    Guid PostId,
    Guid BlogId,
    DateTimeOffset At);

public record PostShared(
    Guid PostId,
    Guid BlogId,
    DateTimeOffset At);

public record PostDeleted(
    Guid PostId,
    DateTimeOffset At);