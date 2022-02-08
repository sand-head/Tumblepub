using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Tumblepub.ActivityPub.Endpoints;

public sealed class GetActorObjectEndpoint : Endpoint
{
    private readonly IActivityPubService _activityPubService;
    private readonly ILogger<GetActorObjectEndpoint> _logger;

    public GetActorObjectEndpoint(IActivityPubService activityPubService, ILogger<GetActorObjectEndpoint> logger)
    {
        _activityPubService = activityPubService ?? throw new ArgumentNullException(nameof(activityPubService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<IActionResult> InvokeAsync(RouteData? routeData, CancellationToken token = default)
    {
        var userId = Guid.Parse(routeData!.Values["userId"]!.ToString()!);
        var objectId = Guid.Parse(routeData!.Values["objectId"]!.ToString()!);
        var @object = await _activityPubService.GetObject(userId, objectId, token);

        if (@object == null)
        {
            _logger.LogInformation("Object {Id} not found.", objectId);
            return NotFound();
        }

        _logger.LogInformation("Object {Id} found.", objectId);
        return ActivityStreams(@object);
    }
}
