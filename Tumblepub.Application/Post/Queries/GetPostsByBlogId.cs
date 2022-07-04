using Tumblepub.Application.Interfaces;

namespace Tumblepub.Application.Post.Queries;

public record GetPostsByBlogIdQuery(Guid BlogId, int Skip, int Take) : IQuery<IEnumerable<Aggregates.Post>>;

internal class GetPostsByBlogIdQueryHandler : IQueryHandler<GetPostsByBlogIdQuery, IEnumerable<Aggregates.Post>>
{
    private readonly IQueryableRepository<Aggregates.Post, Guid> _postRepository;

    public GetPostsByBlogIdQueryHandler(IQueryableRepository<Aggregates.Post, Guid> postRepository)
    {
        _postRepository = postRepository;
    }
    
    public Task<IEnumerable<Aggregates.Post>> Handle(GetPostsByBlogIdQuery query, CancellationToken token = default)
    {
        var (blogId, skip, take) = query;
        var posts = _postRepository.Query()
            .Where(p => p.BlogId == blogId)
            .OrderByDescending(p => p.CreatedAt)
            .Skip(skip)
            .Take(take);

        return Task.FromResult(posts.AsEnumerable());
    }
}