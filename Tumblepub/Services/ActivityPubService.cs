using Tumblepub.ActivityPub;
using Tumblepub.ActivityPub.Models;
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
        if (blog == null) return null;

        return new Actor
        {
            Context = new()
            {
                "https://www.w3.org/ns/activitystreams",
                "https://w3id.org/security/v1"
            },

            // todo: get domain
            Id = $"/actors/{blog.Id}",
            Type = "Person",
            Name = blog.Name,
            PreferredUsername = blog.Title ?? blog.Name,
            Summary = blog.Description,

            InboxUrl = $"/actors/{blog.Id}/inbox",
            FollowersUrl = $"/actors/{blog.Id}/followers",

            PublicKey = new()
            {
                Id = $"/actors/{blog.Id}#main-key",
                Owner = $"/actors/{blog.Id}",
                PublicKeyPem = blog.PublicKey
            }
        };
    }

    public Task GetActorFollowers(Guid id, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
