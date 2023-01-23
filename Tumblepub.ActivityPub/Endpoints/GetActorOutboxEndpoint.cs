using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Tumblepub.ActivityPub.ActivityStreams;
using Tumblepub.ActivityPub.Extensions;
using Tumblepub.Application.Aggregates;
using Tumblepub.Application.Extensions;
using Tumblepub.Application.Interfaces;

namespace Tumblepub.ActivityPub.Endpoints;

public class GetActorOutboxEndpoint : Endpoint
{
    private readonly IQueryableRepository<BlogActivity, Guid> _queryableBlogActivityRepository;
    private readonly IReadOnlyRepository<Blog, Guid> _blogRepository;
    private readonly IReadOnlyRepository<Post, Guid> _postRepository;
    private readonly ILogger<GetActorOutboxEndpoint> _logger;

    public GetActorOutboxEndpoint(IQueryableRepository<BlogActivity, Guid> queryableBlogActivityRepository, IReadOnlyRepository<Blog, Guid> blogRepository, IReadOnlyRepository<Post, Guid> postRepository, ILogger<GetActorOutboxEndpoint> logger)
    {
        _queryableBlogActivityRepository = queryableBlogActivityRepository ?? throw new ArgumentNullException(nameof(queryableBlogActivityRepository));
        _blogRepository = blogRepository ?? throw new ArgumentNullException(nameof(blogRepository));
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<IActionResult> InvokeAsync(RouteData? routeData, CancellationToken token = default)
    {
        var actorId = Guid.Parse(routeData!.Values["actorId"]!.ToString()!);
        var pageQueryString = Context.Request.Query["page"].ToString();

        var outboxUrl = string.Format(ActivityPubConstants.ActorOutboxRoute, actorId);
        int? pageNumber = string.IsNullOrEmpty(pageQueryString) ? null : int.Parse(pageQueryString);

        ActivityStreamsObject outbox;
        if (pageNumber == null)
        {
            outbox = new OrderedCollection
            {
                Id = new(outboxUrl, UriKind.Relative),
                FirstUrl = new($"{outboxUrl}?page=0", UriKind.Relative),
                TotalItems = await _queryableBlogActivityRepository.CountAsync(a => a.BlogId == actorId, token),
            };
        }
        else
        {
            var blogActivities = await _queryableBlogActivityRepository.GetByBlogIdAsync(actorId, pageNumber.Value, token);
            var activities = await Task.WhenAll(blogActivities.Select(async a => await a.ToActivityAsync(_blogRepository, _postRepository, token)));
            
            outbox = new OrderedCollection<Activity>
            {
                Id = new($"{outboxUrl}?page={pageNumber}", UriKind.Relative),
                NextUrl = new($"{outboxUrl}?page={pageNumber + 1}", UriKind.Relative),
                OrderedItems = activities.ToList(),
            };
        }

        if (outbox == null)
        {
            _logger.LogInformation("Outbox for actor {Id} not found.", actorId);
            return NotFound();
        }

        _logger.LogInformation("Outbox for actor {Id} found.", actorId);
        return ActivityStreams(outbox);
    }
}
