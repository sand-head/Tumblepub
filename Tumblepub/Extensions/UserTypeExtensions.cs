using HotChocolate.Resolvers;
using Tumblepub.Database.Models;
using Tumblepub.Database.Repositories;

namespace Tumblepub.Extensions;

[ExtendObjectType(typeof(User),
    IgnoreProperties = new[] { nameof(User.Id), nameof(User.PasswordHash) })]
public class UserTypeExtensions
{
    public async Task<IEnumerable<Blog>> GetBlogs(
        IResolverContext context,
        [Parent] User user,
        [Service] IUserBlogsRepository userBlogsRepository,
        [Service] IBlogRepository blogRepository)
    {
        return await context.GroupDataLoader<Guid, Blog>(
            async (keys, token) =>
            {
                var userBlogs = await userBlogsRepository.GetByIdsAsync(keys, token);
                var allBlogs = await blogRepository.GetByIdsAsync(userBlogs.SelectMany(ub => ub.BlogIds), token);
                return allBlogs.ToLookup(b => b.UserId ?? Guid.Empty);
            })
            .LoadAsync(user.Id);
    }
}
