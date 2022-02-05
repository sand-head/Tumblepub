using Tumblepub.ActivityPub;
using Tumblepub.ActivityPub.Models;
using Tumblepub.Database.Models;
using Tumblepub.Database.Repositories;

namespace Tumblepub.Services;

public class ActivityPubService : IActivityPubService
{
    private readonly IBlogRepository _blogRepository;

    public ActivityPubService(IBlogRepository blogRepository)
    {
        _blogRepository = blogRepository ?? throw new ArgumentNullException(nameof(blogRepository));
    }

    public async Task<Actor?> GetActor(Guid id, CancellationToken token = default)
    {
        var blog = await _blogRepository.GetByIdAsync(id, token);
        if (blog == null)
        {
            return null;
        }

        return MapBlogToActor(blog);
    }

    public async Task<Actor?> GetActorByName(string name, CancellationToken token = default)
    {
        var blog = await _blogRepository.GetByNameAsync(name, null, token);
        if (blog == null)
        {
            return null;
        }

        return MapBlogToActor(blog);
    }

    public Task GetActorFollowers(Guid id, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    private static Actor MapBlogToActor(Blog blog)
    {
        return new Actor
        {
            Context = new()
            {
                "https://www.w3.org/ns/activitystreams",
                "https://w3id.org/security/v1"
            },

            // todo: get domain
            Id = new Uri($"/actors/{blog.Id}", UriKind.Relative),
            Type = "Person",
            Name = blog.Name,
            PreferredUsername = blog.Title ?? blog.Name,
            Summary = blog.Description,

            InboxUrl = new Uri($"/actors/{blog.Id}/inbox", UriKind.Relative),
            FollowersUrl = new Uri($"/actors/{blog.Id}/followers", UriKind.Relative),

            PublicKey = new()
            {
                Id = new Uri($"/actors/{blog.Id}#main-key", UriKind.Relative),
                Owner = new Uri($"/actors/{blog.Id}", UriKind.Relative),
                PublicKeyPem = blog.PublicKey
            }
        };
    }
}
