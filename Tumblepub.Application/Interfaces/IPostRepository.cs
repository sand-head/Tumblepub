using Tumblepub.Application.Models;

namespace Tumblepub.Application.Interfaces;

public interface IPostRepository
{
    Task<Post> CreateExternalPost(Guid blogId, Uri externalPostUrl, CancellationToken token = default);
    Task<Post> CreateMarkdownPost(Guid blogId, string content, IEnumerable<string>? tags, CancellationToken token = default);
    Task<Post?> GetByIdAsync(Guid id, CancellationToken token = default);
}