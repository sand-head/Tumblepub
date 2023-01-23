using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Tumblepub.Application.Aggregates;
using Tumblepub.Application.Interfaces;
using Tumblepub.ActivityPub.Extensions;

namespace Tumblepub.ActivityPub.Endpoints;

public sealed class GetActorActivityEndpoint : Endpoint
{
    private readonly IReadOnlyRepository<BlogActivity, Guid> _blogActivityRepository;
    private readonly IReadOnlyRepository<Blog, Guid> _blogRepository;
    private readonly IReadOnlyRepository<Post, Guid> _postRepository;
    private readonly ILogger<GetActorActivityEndpoint> _logger;

    public GetActorActivityEndpoint(IReadOnlyRepository<BlogActivity, Guid> blogActivityRepository, IReadOnlyRepository<Blog, Guid> blogRepository, IReadOnlyRepository<Post, Guid> postRepository, ILogger<GetActorActivityEndpoint> logger)
    {
        _blogActivityRepository = blogActivityRepository;
        _blogRepository = blogRepository ?? throw new ArgumentNullException(nameof(blogRepository));
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
        _logger = logger;
    }

    public override async Task<IActionResult> InvokeAsync(RouteData? routeData, CancellationToken token = default)
    {
        var actorId = Guid.Parse(routeData!.Values["actorId"]!.ToString()!);
        var activityId = Guid.Parse(routeData!.Values["activityId"]!.ToString()!);
        
        var activity = await _blogActivityRepository.GetByIdAsync(activityId, token);
        if (activity == null || activity.BlogId != actorId)
        {
            _logger.LogInformation("Activity {ActivityId} not found for actor {ActorId}.", activityId, actorId);
            return NotFound();
        }

        _logger.LogInformation("Activity {Id} found.", activityId);
        return ActivityStreams(await activity.ToActivityAsync(_blogRepository, _postRepository, token));
    }
}
