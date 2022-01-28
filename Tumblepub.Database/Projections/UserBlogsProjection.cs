using Marten.Events.Projections;
using Tumblepub.Database.Events;
using Tumblepub.Database.Models;

namespace Tumblepub.Database.Projections;

public class UserBlogsProjection : ViewProjection<UserBlogs, Guid>
{
    public UserBlogsProjection()
    {
        Identity<UserCreated>(u => u.UserId);
        Identity<BlogCreated>(b => b.UserId);

        //DeleteEvent<UserDeleted>();
    }

    public UserBlogs Create(UserCreated @event)
    {
        return new UserBlogs
        {
            Id = @event.UserId,
        };
    }

    public void Apply(UserBlogs state, BlogCreated @event)
    {
        state.BlogIds.Add(@event.BlogId);
    }

    /*
    public void Apply(BlogDeleted @event, UserDto view)
    {
        view.Blogs.Remove(@event.BlogId);
    }
    */
}
