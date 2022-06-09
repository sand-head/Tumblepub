﻿using Tumblepub.Application.Interfaces;
using Tumblepub.Application.Models;

namespace Tumblepub.Application.Extensions;

public static class BlogRepositoryExtensions
{
    public static async Task<Models.Blog?> GetByNameAsync(this IReadOnlyRepository<Models.Blog, BlogId> blogRepository, string name, string? domain, CancellationToken cancellationToken = default)
    {
        // todo: also filter by domain
        return await blogRepository.FirstOrDefaultAsync(b => b.Name == name, cancellationToken);
    }

    public static async Task<IEnumerable<Models.Blog>> GetByIdsAsync(this IQueryableRepository<Models.Blog, BlogId> blogQueryableRepository, IEnumerable<BlogId> ids, CancellationToken cancellationToken = default)
    {
        return blogQueryableRepository.Query()
            .Where(b => ids.Contains(b.Id))
            .ToList();
    }

    public static async Task<IEnumerable<Models.Blog>> GetByUserIdAsync(this IQueryableRepository<Models.Blog, BlogId> blogQueryableRepository, UserId userId, CancellationToken cancellationToken = default)
    {
        return blogQueryableRepository.Query()
            .Where(b => b.UserId.HasValue && b.UserId.Value == userId)
            .ToList();
    }
}