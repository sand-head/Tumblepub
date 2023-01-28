using MediatR;
using Tumblepub.Application.Interfaces;

namespace Tumblepub.Application.Blog.Queries;

public record GetBlogByIdsQuery(IEnumerable<Guid> Ids) : IRequest<IEnumerable<Aggregates.Blog>>;

internal class GetBlogByIdsQueryHandler : IRequestHandler<GetBlogByIdsQuery, IEnumerable<Aggregates.Blog>>
{
    private readonly IQueryableRepository<Aggregates.Blog, Guid> _blogRepository;

    public GetBlogByIdsQueryHandler(IQueryableRepository<Aggregates.Blog, Guid> blogRepository)
    {
        _blogRepository = blogRepository;
    }

    public Task<IEnumerable<Aggregates.Blog>> Handle(GetBlogByIdsQuery query, CancellationToken token = default)
    {
        return Task.FromResult<IEnumerable<Aggregates.Blog>>(_blogRepository.Query()
            .Where(b => query.Ids.Contains(b.Id))
            .ToList());
    }
}