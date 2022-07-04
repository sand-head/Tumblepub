﻿using Tumblepub.Application.Interfaces;

namespace Tumblepub.Application.Blog.Queries;

public record GetBlogByNameQuery(string Name, string? Domain = null) : IQuery<Aggregates.Blog?>;

internal class GetBlogByNameQueryHandler : IQueryHandler<GetBlogByNameQuery, Aggregates.Blog?>
{
    private readonly IReadOnlyRepository<Aggregates.Blog, Guid> _blogRepository;

    public GetBlogByNameQueryHandler(IReadOnlyRepository<Aggregates.Blog, Guid> blogRepository)
    {
        _blogRepository = blogRepository;
    }
    
    public async Task<Aggregates.Blog?> Handle(GetBlogByNameQuery query, CancellationToken token = default)
    {
        var (name, domain) = query;
        // todo: also filter by domain
        return await _blogRepository.FirstOrDefaultAsync(b => b.Name == name, token);
    }
}