using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Tumblepub.ActivityPub.Endpoints;

public sealed class PostActorInboxEndpoint : Endpoint
{
    public override Task<IActionResult> InvokeAsync(RouteData? routeData, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
