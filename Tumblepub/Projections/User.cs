using Tumblepub.Events;

namespace Tumblepub.Projections;

public class User
{
    public Guid Id { get; private set; }
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public bool IsDeleted { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    public int Version { get; private set; }

    public void Apply(UserCreated @event)
    {
        Id = @event.UserId;
        Email = @event.Email;
        PasswordHash = @event.PasswordHash;
        IsDeleted = false;
        UpdatedAt = CreatedAt = @event.At;
        Version++;
    }

    public void Apply(UserDeleted @event)
    {
        IsDeleted = true;
        UpdatedAt = @event.At;
        Version++;
    }
}