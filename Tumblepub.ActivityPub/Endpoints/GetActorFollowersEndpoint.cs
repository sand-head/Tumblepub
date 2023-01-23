using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Tumblepub.ActivityPub.ActivityStreams;

namespace Tumblepub.ActivityPub.Endpoints;

public sealed class GetActorFollowersEndpoint : Endpoint
{
    private readonly ILogger<GetActorFollowersEndpoint> _logger;

    public GetActorFollowersEndpoint(ILogger<GetActorFollowersEndpoint> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<IActionResult> InvokeAsync(RouteData? routeData, CancellationToken token = default)
    {
        var actorId = Guid.Parse(routeData!.Values["actorId"]!.ToString()!);
        var followersUrl = string.Format(ActivityPubConstants.ActorFollowersRoute, actorId);
        
        // todo: actually implement this
        var followers = new Collection()
        {
            Id = new(followersUrl, UriKind.Relative),
        };

        if (followers == null)
        {
            _logger.LogInformation("Followers for actor {Id} not found.", actorId);
            return NotFound();
        }

        _logger.LogInformation("Followers for actor {Id} found.", actorId);
        return ActivityStreams(followers);
    }
}
