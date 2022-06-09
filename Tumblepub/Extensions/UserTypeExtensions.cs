using HotChocolate.Resolvers;
using Tumblepub.Application.Extensions;
using Tumblepub.Application.Interfaces;
using Tumblepub.Application.Models;

namespace Tumblepub.Extensions;

[ExtendObjectType(typeof(User),
    IgnoreProperties = new[] { nameof(User.Id), nameof(User.PasswordHash) })]
public class UserTypeExtensions
{
    public async Task<IEnumerable<Blog>> GetBlogs(
        IResolverContext context,
        [Parent] User user,
        [Service] IQueryableRepository<Blog, BlogId> blogRepository)
    {
        return await context.GroupDataLoader<UserId, Blog>(
            async (userIds, token) =>
            {
                var getBlogsResults = await Task.WhenAll(userIds.Select(async id => await blogRepository.GetByUserIdAsync(id, token)));
                return getBlogsResults.SelectMany(r => r).ToLookup(b => b.UserId!.Value);
            })
            .LoadAsync(user.Id);
    }
}
