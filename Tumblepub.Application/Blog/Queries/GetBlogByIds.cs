using Tumblepub.Application.Interfaces;

namespace Tumblepub.Application.Blog.Queries;

public record GetBlogByIdsQuery(IEnumerable<Guid> Ids) : IQuery<IEnumerable<Models.Blog>>;

internal class GetBlogByIdsQueryHandler : IQueryHandler<GetBlogByIdsQuery, IEnumerable<Models.Blog>>
{
    private readonly IQueryableRepository<Models.Blog, Guid> _blogRepository;

    public GetBlogByIdsQueryHandler(IQueryableRepository<Models.Blog, Guid> blogRepository)
    {
        _blogRepository = blogRepository;
    }

    public Task<IEnumerable<Models.Blog>> Handle(GetBlogByIdsQuery query, CancellationToken token = default)
    {
        return Task.FromResult<IEnumerable<Models.Blog>>(_blogRepository.Query()
            .Where(b => query.Ids.Contains(b.Id))
            .ToList());
    }
}