using Tumblepub.Application.Interfaces;

namespace Tumblepub.Application.Blog.Queries;

public record GetBlogByNameQuery(string Name, string? Domain = null) : IQuery<Models.Blog?>;

internal class GetBlogByNameQueryHandler : IQueryHandler<GetBlogByNameQuery, Models.Blog?>
{
    private readonly IReadOnlyRepository<Models.Blog, Guid> _blogRepository;

    public GetBlogByNameQueryHandler(IReadOnlyRepository<Models.Blog, Guid> blogRepository)
    {
        _blogRepository = blogRepository;
    }
    
    public async Task<Models.Blog?> Handle(GetBlogByNameQuery query, CancellationToken token = default)
    {
        var (name, domain) = query;
        // todo: also filter by domain
        return await _blogRepository.FirstOrDefaultAsync(b => b.Name == name, token);
    }
}