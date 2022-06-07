using Tumblepub.Application.Models;

namespace Tumblepub.Application.Events;

public record PostCreated(
    PostId PostId,
    BlogId BlogId,
    PostContent Content,
    DateTimeOffset At);

public record PostDiscovered(
    PostId PostId,
    BlogId BlogId,
    DateTimeOffset At);

public record PostUpdated(
    PostId PostId,
    DateTimeOffset At);

public record PostLiked(
    PostId PostId,
    BlogId BlogId,
    DateTimeOffset At);

public record PostShared(
    PostId PostId,
    BlogId BlogId,
    DateTimeOffset At);

public record PostDeleted(
    PostId PostId,
    DateTimeOffset At);