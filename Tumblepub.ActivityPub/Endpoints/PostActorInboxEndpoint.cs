using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Text.Json;
using Tumblepub.ActivityPub.ActivityStreams;

namespace Tumblepub.ActivityPub.Endpoints;

public sealed class PostActorInboxEndpoint : Endpoint
{
    public override async Task<IActionResult> InvokeAsync(RouteData? routeData, CancellationToken token = default)
    {
        var activity = await JsonSerializer.DeserializeAsync<Activity>(Context.Request.Body, cancellationToken: token);
        // todo: handle activities
        throw new NotImplementedException();
    }
}
