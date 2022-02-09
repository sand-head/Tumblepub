using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Tumblepub.ActivityPub.Endpoints;

public sealed class GetActorActivityEndpoint : Endpoint
{
    private readonly IActivityPubService _activityPubService;
    private readonly ILogger<GetActorActivityEndpoint> _logger;

    public GetActorActivityEndpoint(IActivityPubService activityPubService, ILogger<GetActorActivityEndpoint> logger)
    {
        _activityPubService = activityPubService ?? throw new ArgumentNullException(nameof(activityPubService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<IActionResult> InvokeAsync(RouteData? routeData, CancellationToken token = default)
    {
        var userId = Guid.Parse(routeData!.Values["userId"]!.ToString()!);
        var activityId = Guid.Parse(routeData!.Values["activityId"]!.ToString()!);
        var activity = await _activityPubService.GetActivity(userId, activityId, token);

        if (activity == null)
        {
            _logger.LogInformation("Activity {Id} not found.", activityId);
            return NotFound();
        }

        _logger.LogInformation("Activity {Id} found.", activityId);
        return ActivityStreams(activity);
    }
}
