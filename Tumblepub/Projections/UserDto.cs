using Marten.Events.Projections;
using Tumblepub.Events;
using Tumblepub.Services;

namespace Tumblepub.Projections;

public class UserDto
{
    [GraphQLIgnore]
    public Guid Id { get; set; }
    public string Email { get; set; } = default!;
    public List<Guid> Blogs { get; set; } = new();

    #region GraphQL resolvers

    /*
    public async Task<IEnumerable<BlogDto>> GetBlogs([Service] IBlogService blogService)
    {

    }
    */

    #endregion
}

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
