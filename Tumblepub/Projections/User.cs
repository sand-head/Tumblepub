using Marten.Events.Aggregation;
using Tumblepub.Events;

namespace Tumblepub.Projections;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public int Version { get; set; }
}

public class UserProjection : AggregateProjection<User>
{
    public UserProjection()
    {
        DeleteEvent<UserDeleted>();
    }

    public User Create(UserCreated e)
    {
        return new User
        {
            Id = e.UserId,
            Email = e.Email,
            PasswordHash = e.PasswordHash,
            UpdatedAt = e.At,
            CreatedAt = e.At
        };
    }
}