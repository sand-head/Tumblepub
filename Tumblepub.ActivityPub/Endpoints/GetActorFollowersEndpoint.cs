using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Tumblepub.ActivityPub.Endpoints;

public sealed class GetActorFollowersEndpoint : Endpoint
{
    private readonly IActivityPubService _activityPubService;
    private readonly ILogger<GetActorFollowersEndpoint> _logger;

    public GetActorFollowersEndpoint(IActivityPubService activityPubService, ILogger<GetActorFollowersEndpoint> logger)
    {
        _activityPubService = activityPubService ?? throw new ArgumentNullException(nameof(activityPubService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<IActionResult> InvokeAsync(RouteData? routeData, CancellationToken token = default)
    {
        var userId = Guid.Parse(routeData!.Values["userId"]!.ToString()!);
        var followers = await _activityPubService.GetActorFollowers(userId, token);

        if (followers == null)
        {
            _logger.LogInformation("Followers for actor {Id} not found.", userId);
            return NotFound();
        }

        _logger.LogInformation("Followers for actor {Id} found.", userId);
        return ActivityStreams(followers);
    }
}
