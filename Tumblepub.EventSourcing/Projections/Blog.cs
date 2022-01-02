using Tumblepub.Database.Events;

namespace Tumblepub.Database.Projections;

public class Blog
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string PublicKey { get; private set; } = string.Empty;
    public string? PrivateKey { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public int Version { get; private set; }

    public void Apply(BlogCreated @event)
    {
        Id = @event.BlogId;
        Name = @event.BlogName;
        PublicKey = @event.PublicKey;
        PrivateKey = @event.PrivateKey;
        IsDeleted = false;
        UpdatedAt = CreatedAt = @event.At;
        Version++;
    }

    public void Apply(BlogDiscovered @event)
    {
        Id = @event.BlogId;
        Name = @event.BlogName;
        PublicKey = @event.PublicKey;
        IsDeleted = false;
        UpdatedAt = CreatedAt = @event.At;
        Version++;
    }

    public void Apply(BlogDeleted @event)
    {
        IsDeleted = true;
        UpdatedAt = @event.At;
    }
}
