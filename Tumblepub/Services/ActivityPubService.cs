using Tumblepub.ActivityPub;
using Tumblepub.ActivityPub.ActivityStreams;
using Tumblepub.Application.Blog.Queries;
using Tumblepub.Application.Extensions;
using Tumblepub.Application.Interfaces;
using Tumblepub.Application.Models;
using Object = Tumblepub.ActivityPub.ActivityStreams.Object;
using ObjectType = Tumblepub.Application.Models.ObjectType;

namespace Tumblepub.Services;

public class ActivityPubService : IActivityPubService
{
    private readonly ActivityPubOptions _options;
    private readonly IQueryHandler<GetBlogByNameQuery, Blog?> _getBlogByNameQueryHandler;
    private readonly IReadOnlyRepository<Blog, Guid> _blogRepository;
    private readonly IReadOnlyRepository<Post, Guid> _postRepository;
    private readonly IReadOnlyRepository<BlogActivity, Guid> _blogActivityRepository;
    private readonly IQueryableRepository<BlogActivity, Guid> _queryableBlogActivityRepository;

    public ActivityPubService(
        ActivityPubOptions options,
        IQueryHandler<GetBlogByNameQuery, Blog?> getBlogByNameQueryHandler,
        IReadOnlyRepository<Blog, Guid> blogRepository,
        IReadOnlyRepository<Post, Guid> postRepository,
        IReadOnlyRepository<BlogActivity, Guid> blogActivityRepository,
        IQueryableRepository<BlogActivity, Guid> queryableBlogActivityRepository)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _getBlogByNameQueryHandler = getBlogByNameQueryHandler ?? throw new ArgumentNullException(nameof(getBlogByNameQueryHandler));
        _blogRepository = blogRepository ?? throw new ArgumentNullException(nameof(blogRepository));
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
        _blogActivityRepository = blogActivityRepository ?? throw new ArgumentNullException(nameof(blogActivityRepository));
        _queryableBlogActivityRepository = queryableBlogActivityRepository ?? throw new ArgumentNullException(nameof(queryableBlogActivityRepository));
    }

    public async Task<Actor?> GetActor(Guid id, CancellationToken token = default)
    {
        var blog = await _blogRepository.GetByIdAsync(id, token);
        return blog == null ? null : MapBlogToActor(blog);
    }

    public async Task<Actor?> GetActorByName(string name, CancellationToken token = default)
    {
        var query = new GetBlogByNameQuery(name);
        var blog = await _getBlogByNameQueryHandler.Handle(query, token);
        return blog == null ? null : MapBlogToActor(blog);
    }

    public async Task<Activity?> GetActivity(Guid actorId, Guid activityId, CancellationToken token = default)
    {
        var blogActivity = await _blogActivityRepository.GetByIdAsync(activityId, token);
        if (blogActivity == null || blogActivity.BlogId != actorId)
        {
            return null;
        }

        return await MapBlogActivityToActivity(blogActivity, token);
    }

    public async Task<Object?> GetObject(Guid actorId, Guid objectId, CancellationToken token = default)
    {
        var post = await _postRepository.GetByIdAsync(objectId, token);
        if (post == null || post.BlogId != actorId)
        {
            return null;
        }

        var mappedPost = MapPostToObject(post);
        return mappedPost as Object ?? null;
    }

    public async Task<Object?> GetOutbox(Guid actorId, int? pageNumber = null, CancellationToken token = default)
    {
        var outboxUrl = string.Format(_options.ActorOutboxRouteTemplate, actorId);

        if (pageNumber == null)
        {
            return new OrderedCollection
            {
                Id = new(outboxUrl, UriKind.Relative),
                FirstUrl = new($"{outboxUrl}?page=0", UriKind.Relative),
                TotalItems = await _queryableBlogActivityRepository.CountAsync(a => a.BlogId == actorId, token),
            };
        }

        var blogActivities = await _queryableBlogActivityRepository.GetByBlogIdAsync(actorId, pageNumber.Value, token);
        var activities = await Task.WhenAll(blogActivities.Select(async a => await MapBlogActivityToActivity(a)));

        return new OrderedCollection<Activity>
        {
            Id = new($"{outboxUrl}?page={pageNumber}", UriKind.Relative),
            NextUrl = new($"{outboxUrl}?page={pageNumber + 1}", UriKind.Relative),
            OrderedItems = activities.ToList(),
        };
    }

    private Actor MapBlogToActor(Blog blog)
    {
        var actorId = string.Format(_options.ActorRouteTemplate, blog.Id);
        return new("Person")
        {
            Context = new List<string>()
            {
                "https://www.w3.org/ns/activitystreams"
            },

            // todo: get domain
            Id = new(actorId, UriKind.Relative),
            Name = blog.Name,
            PublishedAt = blog.CreatedAt,
            PreferredUsername = blog.Title ?? blog.Name,
            Summary = blog.Description,

            InboxUrl = new(string.Format(_options.ActorInboxRouteTemplate, blog.Id), UriKind.Relative),
            FollowersUrl = new(string.Format(_options.ActorFollowersRouteTemplate, blog.Id), UriKind.Relative),

            PublicKey = new()
            {
                Id = new($"{actorId}#main-key", UriKind.Relative),
                Owner = new(actorId, UriKind.Relative),
                PublicKeyPem = blog.PublicKey
            }
        };
    }

    private ActivityStreamsValue MapPostToObject(Post post)
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

            Id = new(string.Format(_options.ActorObjectRouteTemplate, post.BlogId, post.Id), UriKind.Relative),
            PublishedAt = post.CreatedAt,
            AttributedTo = new()
            {
                new Link(new(string.Format(_options.ActorRouteTemplate, post.BlogId), UriKind.Relative))
            },

            To = new()
            {
                new Link(new("https://www.w3.org/ns/activitystreams#Public"))
            }
        };

        return post.Content switch
        {
            PostContent.Markdown textContent => postObject with
            {
                Content = textContent.Content
            },
            _ => postObject
        };
    }

    private async Task<Activity> MapBlogActivityToActivity(BlogActivity blogActivity, CancellationToken token = default)
    {
        return new(blogActivity.Type)
        {
            Context = new List<string>()
            {
                "https://www.w3.org/ns/activitystreams"
            },

            Id = new(string.Format(_options.ActorActivityRouteTemplate, blogActivity.BlogId, blogActivity.Id), UriKind.Relative),
            Actor = new List<ActivityStreamsValue>
            {
                new Link(new(string.Format(_options.ActorRouteTemplate, blogActivity.BlogId), UriKind.Relative))
            },
            PublishedAt = blogActivity.PublishedAt,

            Object = blogActivity.ObjectType switch
            {
                // maybe this kind of sucks and I should just make these links
                ObjectType.Blog => MapBlogToActor((await _blogRepository.GetByIdAsync(blogActivity.ObjectId!.Value, token))!),
                ObjectType.Post => MapPostToObject((await _postRepository.GetByIdAsync(blogActivity.ObjectId!.Value, token))!),
                _ => null
            },
        };
    }
}
