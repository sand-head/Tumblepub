using Marten.Events.Projections;
using Marten.Schema;
using Tumblepub.Database.Events;
using Tumblepub.Database.Models;

namespace Tumblepub.Database.Projections;

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

public class UserBlogsProjection : ViewProjection<UserBlogs, Guid>
{
    public UserBlogsProjection()
    {
        Identity<UserCreated>(u => u.UserId);
        Identity<BlogCreated>(b => b.UserId);
    }

    public void Apply(UserCreated @event, UserBlogs view)
    {
        view.UserEmail = @event.Email;
    }

    public void Apply(BlogCreated @event, UserBlogs view)
    {
        view.Blogs.Add(@event.BlogId);
    }

    public void Apply(BlogDeleted @event, UserBlogs view)
    {
        view.Blogs.Remove(@event.BlogId);
    }
}