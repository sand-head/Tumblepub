using AutoMapper;
using HotChocolate.Resolvers;
using Tumblepub.Application.Aggregates;
using Tumblepub.Application.Blog.Queries;
using Tumblepub.Application.Interfaces;

namespace Tumblepub.Models;

[GraphQLName("User")]
public class UserDto
{
    [GraphQLIgnore]
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    [GraphQLIgnore]
    public string PasswordHash { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    
    public async Task<IEnumerable<BlogDto>> GetBlogs(
        IResolverContext context,
        [Service] IMapper mapper,
        [Service] IQueryHandler<GetBlogsByUserIdQuery, IEnumerable<Blog>> queryHandler)
    {
        return await context.GroupDataLoader<Guid, BlogDto>(
                async (userIds, token) =>
                {
                    var getBlogsResults = await Task.WhenAll(userIds.Select(async id =>
                    {
                        var query = new GetBlogsByUserIdQuery(id);
                        return await queryHandler.Handle(query, token);
                    }));
                    
                    return getBlogsResults
                        .SelectMany(r => r.Select(b => mapper.Map<BlogDto>(b)))
                        .ToLookup(b => b.UserId!.Value);
                })
            .LoadAsync(Id);
    }
}