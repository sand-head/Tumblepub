using Tumblepub.ActivityPub;
using Tumblepub.ActivityPub.ActivityStreams;
using Tumblepub.Database.Models;
using Tumblepub.Database.Repositories;
using Object = Tumblepub.ActivityPub.ActivityStreams.Object;

namespace Tumblepub.Services;

public class ActivityPubService : IActivityPubService
{
    private readonly ActivityPubOptions _options;
    private readonly IBlogRepository _blogRepository;
    private readonly IPostRepository _postRepository;
    private readonly IBlogActivityRepository _blogActivityRepository;

    public ActivityPubService(
        ActivityPubOptions options,
        IBlogRepository blogRepository,
        IPostRepository postRepository,
        IBlogActivityRepository blogActivityRepository)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _blogRepository = blogRepository ?? throw new ArgumentNullException(nameof(blogRepository));
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
        _blogActivityRepository = blogActivityRepository ?? throw new ArgumentNullException(nameof(blogActivityRepository));
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

    public async Task<Activity?> GetActivity(Guid actorId, Guid activityId, CancellationToken token = default)
    {
        var blogActivity = await _blogActivityRepository.GetByIdAsync(activityId, token);
        if (blogActivity == null || blogActivity.BlogId != actorId)
        {
            return null;
        }

        return new Activity(blogActivity.Type)
        {
            Id = new Uri(string.Format(_options.ActorActivityRouteTemplate, blogActivity.BlogId, blogActivity.Id), UriKind.Relative),
            Actor = new List<ActivityStreamsValue>
            {
                new Link(new Uri(string.Format(_options.ActorRouteTemplate, blogActivity.BlogId), UriKind.Relative))
            },
            PublishedAt = blogActivity.PublishedAt,
        };
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

    private Actor MapBlogToActor(Blog blog)
    {
        var actorId = string.Format(_options.ActorRouteTemplate, blog.Id);
        return new Actor("Person")
        {
            Context = new List<string>()
            {
                "https://www.w3.org/ns/activitystreams"
            },

            // todo: get domain
            Id = new Uri(actorId, UriKind.Relative),
            Name = blog.Name,
            PublishedAt = blog.CreatedAt,
            PreferredUsername = blog.Title ?? blog.Name,
            Summary = blog.Description,

            InboxUrl = new Uri(string.Format(_options.ActorInboxRouteTemplate, blog.Id), UriKind.Relative),
            FollowersUrl = new Uri(string.Format(_options.ActorFollowersRouteTemplate, blog.Id), UriKind.Relative),

            PublicKey = new()
            {
                Id = new Uri($"{actorId}#main-key", UriKind.Relative),
                Owner = new Uri(actorId, UriKind.Relative),
                PublicKeyPem = blog.PublicKey
            }
        };
    }

    private ActivityStreamsValue MapPost(Post post)
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

            Id = new Uri(string.Format(_options.ActorObjectRouteTemplate, post.BlogId, post.Id), UriKind.Relative),
            AttributedTo = new()
            {
                new Link(new Uri(string.Format(_options.ActorRouteTemplate, post.BlogId), UriKind.Relative))
            },

            To = new()
            {
                new Link(new Uri("https://www.w3.org/ns/activitystreams#Public"))
            }
        };

        if (post.Content is PostContent.Markdown textContent)
        {
            postObject = postObject with
            {
                Content = textContent.Content
            };
        }

        return postObject;
    }
}
