using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tumblepub.Database.Models;

namespace Tumblepub.Database.Repositories;

public interface IPostRepository
{
    Task<Post> CreateExternalPost(Guid blogId, Uri externalPostUrl, CancellationToken token = default);
    Task<Post> CreateTextPost(Guid blogId, string content, string? title, IEnumerable<string>? tags, CancellationToken token = default);
    Task<Post?> GetPost(Guid id, CancellationToken token = default);
}

public class PostRepository : IPostRepository
{
    public Task<Post> CreateExternalPost(Guid blogId, Uri externalPostUrl, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<Post> CreateTextPost(Guid blogId, string content, string? title, IEnumerable<string>? tags, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public Task<Post?> GetPost(Guid id, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
