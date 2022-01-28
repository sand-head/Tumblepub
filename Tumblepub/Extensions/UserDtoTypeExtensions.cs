using Tumblepub.Database.Models;
using Tumblepub.Database.Repositories;

namespace Tumblepub.Extensions;

[ExtendObjectType(typeof(UserDto),
    IgnoreProperties = new[] { nameof(UserDto.Id), nameof(UserDto.Blogs) })]
public class UserDtoTypeExtensions
{
    public async Task<IEnumerable<Blog>> GetBlogs([Parent] UserDto user, [Service] IBlogRepository blogRepository)
    {
        var blogs = await Task.WhenAll(user.Blogs.Select(async b => await blogRepository.GetByIdAsync(b)));
        return blogs.Where(b => b != null)!;
    }
}
