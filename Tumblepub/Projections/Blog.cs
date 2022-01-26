using System.Text.Json;
using Tumblepub.Events;

namespace Tumblepub.Projections;

public class Blog
{
    public Guid Id { get; private set; }
    public Guid? UserId { get; private set; }
    public string BlogName { get; private set; } = default!;
    public string? Title { get; private set; }
    public string? Description { get; private set; }
    public JsonElement? Metadata { get; private set; }
    public string PublicKey { get; private set; } = default!;
    public string? PrivateKey { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public int Version { get; private set; }

    #region Event handlers

    public void Apply(BlogCreated @event)
    {
        Id = @event.BlogId;
        UserId = @event.UserId;
        BlogName = @event.BlogName;
        PublicKey = @event.PublicKey;
        PrivateKey = @event.PrivateKey;
        IsDeleted = false;
        UpdatedAt = CreatedAt = @event.At;
        Version++;
    }

    public void Apply(BlogDiscovered @event)
    {
        Id = @event.BlogId;
        BlogName = @event.BlogName;
        PublicKey = @event.PublicKey;
        IsDeleted = false;
        UpdatedAt = CreatedAt = @event.At;
        Version++;
    }

    public void Apply(BlogMetadataUpdated @event)
    {
        Title = @event.Title ?? Title;
        Description = @event.Description ?? Description;
        Metadata = @event.Metadata ?? Metadata;
        Version++;
    }

    public void Apply(BlogDeleted @event)
    {
        IsDeleted = true;
        UpdatedAt = @event.At;
    }

    #endregion
}