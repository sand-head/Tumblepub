using Tumblepub.Application.Interfaces;

namespace Tumblepub.Application.Blog.Queries;

public record GetBlogsByUserIdQuery(Guid UserId) : IQuery<IEnumerable<Models.Blog>>;

internal class GetBlogsByUserIdQueryHandler : IQueryHandler<GetBlogsByUserIdQuery, IEnumerable<Models.Blog>>
{
    private readonly IQueryableRepository<Models.Blog, Guid> _blogRepository;

    public GetBlogsByUserIdQueryHandler(IQueryableRepository<Models.Blog, Guid> blogRepository)
    {
        _blogRepository = blogRepository;
    }

    public Task<IEnumerable<Models.Blog>> Handle(GetBlogsByUserIdQuery query, CancellationToken token = default)
    {
        return Task.FromResult<IEnumerable<Models.Blog>>(_blogRepository.Query()
            .Where(b => b.UserId.HasValue && b.UserId.Value == query.UserId)
            .ToList());
    }
}