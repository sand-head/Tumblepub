using Tumblepub.Application.Aggregates;

namespace Tumblepub.Application.Events;

public record PostCreated(
    Guid PostId,
    Guid BlogId,
    PostContent Content,
    DateTimeOffset At);

public record PostDiscovered(
    Guid PostId,
    Guid BlogId,
    Uri ExternalId,
    DateTimeOffset At);

public record PostUpdated(
    Guid PostId,
    Guid BlogId,
    DateTimeOffset At);

public record PostLiked(
    Guid PostId,
    Guid BlogId,
    DateTimeOffset At);

public record PostShared(
    Guid NewPostId,
    Guid BlogId,
    Guid OriginalPostId,
    DateTimeOffset At);

public record PostDeleted(
    Guid PostId,
    Guid BlogId,
    DateTimeOffset At);