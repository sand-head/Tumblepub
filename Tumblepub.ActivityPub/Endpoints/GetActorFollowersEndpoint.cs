using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Tumblepub.ActivityPub.Endpoints;

public sealed class GetActorFollowersEndpoint : Endpoint
{
    private readonly IActivityPubService _activityPubService;

    public GetActorFollowersEndpoint(IActivityPubService activityPubService)
    {
        _activityPubService = activityPubService ?? throw new ArgumentNullException(nameof(activityPubService));
    }

    public override async Task<IActionResult> InvokeAsync(HttpContext context, RouteData? routeData, CancellationToken token = default)
    {
        return NotFound();
    }
}
