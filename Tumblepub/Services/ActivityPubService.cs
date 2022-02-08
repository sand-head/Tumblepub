using Tumblepub.ActivityPub;
using Tumblepub.ActivityPub.ActivityStreams;
using Tumblepub.Database.Models;
using Tumblepub.Database.Repositories;
using Object = Tumblepub.ActivityPub.ActivityStreams.Object;

namespace Tumblepub.Services;

public class ActivityPubService : IActivityPubService
{
    private readonly IBlogRepository _blogRepository;
    private readonly IPostRepository _postRepository;

    public ActivityPubService(IBlogRepository blogRepository, IPostRepository postRepository)
    {
        _blogRepository = blogRepository ?? throw new ArgumentNullException(nameof(blogRepository));
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
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

    public Task<Activity> GetActivity(Guid actorId, Guid activityId, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Object?> GetObject(Guid actorId, Guid objectId, CancellationToken token = default)
    {
        var post = await _postRepository.GetPost(objectId, token);
        if (post == null || post.BlogId != actorId)
        {
            return null;
        }

        var mappedPost = MapPost(post);
        if (mappedPost is not Object mappedPostObject)
        {
            return null;
        }
        return mappedPostObject;
    }

    private static Actor MapBlogToActor(Blog blog)
    {
        return new Actor("Person")
        {
            Context = new List<string>()
            {
                "https://www.w3.org/ns/activitystreams"
            },

            // todo: get domain
            Id = new Uri($"/actors/{blog.Id}", UriKind.Relative),
            Name = blog.Name,
            PublishedAt = blog.CreatedAt,
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

    private static ActivityStreamsValue MapPost(Post post)
    {
        if (post.Content is PostContent.External externalContent)
        {
            return new Link(externalContent.ExternalId);
        }

        // I should simplify how these objects are constructed a bit...
        var postObject = new Object("Note")
        {
            Context = new List<string>()
            {
                "https://www.w3.org/ns/activitystreams"
            },

            Id = new Uri($"/actors/{post.BlogId}/objects/{post.Id}", UriKind.Relative),
            AttributedTo = new()
            {
                new Link(new Uri($"/actors/{post.BlogId}", UriKind.Relative))
            },

            To = new()
            {
                new Link(new Uri("https://www.w3.org/ns/activitystreams#Public"))
            }
        };

        if (post.Content is PostContent.Text textContent)
        {
            postObject = postObject with
            {
                Content = textContent.Content
            };
        }

        return postObject;
    }
}
