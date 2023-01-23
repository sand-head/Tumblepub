using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Tumblepub.ActivityPub.Extensions;
using Tumblepub.Application.Aggregates;
using Tumblepub.Application.Interfaces;

namespace Tumblepub.ActivityPub.Endpoints;

public sealed class GetActorObjectEndpoint : Endpoint
{
    private readonly IReadOnlyRepository<Post, Guid> _postRepository;
    private readonly ILogger<GetActorObjectEndpoint> _logger;

    public GetActorObjectEndpoint(IReadOnlyRepository<Post, Guid> postRepository, ILogger<GetActorObjectEndpoint> logger)
    {
        _postRepository = postRepository ?? throw new ArgumentNullException(nameof(postRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<IActionResult> InvokeAsync(RouteData? routeData, CancellationToken token = default)
    {
        var actorId = Guid.Parse(routeData!.Values["actorId"]!.ToString()!);
        var objectId = Guid.Parse(routeData!.Values["objectId"]!.ToString()!);
        
        var post = await _postRepository.GetByIdAsync(objectId, token);
        if (post.BlogId != actorId)
        {
            post = null;
        }

        var postObject = post?.ToObject();
        if (postObject == null)
        {
            _logger.LogInformation("Object {Id} not found.", objectId);
            return NotFound();
        }

        _logger.LogInformation("Object {Id} found.", objectId);
        return ActivityStreams(postObject);
    }
}
