using HotChocolate.Resolvers;
using Tumblepub.Application.Aggregates;
using Tumblepub.Application.Blog.Queries;
using Tumblepub.Application.Extensions;
using Tumblepub.Application.Interfaces;

namespace Tumblepub.Extensions;

[ExtendObjectType(typeof(User),
    IgnoreProperties = new[] { nameof(User.Id), nameof(User.PasswordHash) })]
public class UserTypeExtensions
{
    public async Task<IEnumerable<Blog>> GetBlogs(
        IResolverContext context,
        [Parent] User user,
        [Service] IQueryHandler<GetBlogsByUserIdQuery, IEnumerable<Blog>> queryHandler)
    {
        return await context.GroupDataLoader<Guid, Blog>(
            async (userIds, token) =>
            {
                var getBlogsResults = await Task.WhenAll(userIds.Select(async id =>
                {
                    var query = new GetBlogsByUserIdQuery(id);
                    return await queryHandler.Handle(query, token);
                }));
                return getBlogsResults.SelectMany(r => r).ToLookup(b => b.UserId!.Value);
            })
            .LoadAsync(user.Id);
    }
}
