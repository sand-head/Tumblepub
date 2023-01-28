using MediatR;
using Tumblepub.Application.Interfaces;

namespace Tumblepub.Application.Blog.Queries;

public record GetBlogsByUserIdQuery(Guid UserId) : IRequest<IEnumerable<Aggregates.Blog>>;

internal class GetBlogsByUserIdQueryHandler : IRequestHandler<GetBlogsByUserIdQuery, IEnumerable<Aggregates.Blog>>
{
    private readonly IQueryableRepository<Aggregates.Blog, Guid> _blogRepository;

    public GetBlogsByUserIdQueryHandler(IQueryableRepository<Aggregates.Blog, Guid> blogRepository)
    {
        _blogRepository = blogRepository;
    }

    public Task<IEnumerable<Aggregates.Blog>> Handle(GetBlogsByUserIdQuery query, CancellationToken token = default)
    {
        return Task.FromResult<IEnumerable<Aggregates.Blog>>(_blogRepository.Query()
            .Where(b => b.UserId.HasValue && b.UserId.Value == query.UserId)
            .ToList());
    }
}