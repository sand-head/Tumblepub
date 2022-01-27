using Marten.Events.Projections;
using Tumblepub.Database.Events;
using Tumblepub.Database.Models;

namespace Tumblepub.Database.Projections;

public class UserDtoProjection : ViewProjection<UserDto, Guid>
{
    public UserDtoProjection()
    {
        Identity<UserCreated>(u => u.UserId);
        Identity<BlogCreated>(b => b.UserId);

        //DeleteEvent<UserDeleted>();
    }

    public UserDto Create(UserCreated @event)
    {
        return new UserDto
        {
            Id = @event.UserId,
            Email = @event.Email,
        };
    }

    public void Apply(UserDto state, BlogCreated @event)
    {
        state.Blogs.Add(@event.BlogId);
    }

    /*
    public void Apply(BlogDeleted @event, UserDto view)
    {
        view.Blogs.Remove(@event.BlogId);
    }
    */
}
