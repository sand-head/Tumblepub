using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Tumblepub.ActivityPub.Extensions;
using Tumblepub.Application.Aggregates;
using Tumblepub.Application.Interfaces;

namespace Tumblepub.ActivityPub.Endpoints;

public sealed class GetActorEndpoint : Endpoint
{
    private readonly ILogger<GetActorEndpoint> _logger;
    private readonly IReadOnlyRepository<Blog, Guid> _blogRepository;

    public GetActorEndpoint(IReadOnlyRepository<Blog, Guid> blogRepository, ILogger<GetActorEndpoint> logger)
    {
        _blogRepository = blogRepository ?? throw new ArgumentNullException(nameof(blogRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<IActionResult> InvokeAsync(RouteData? routeData, CancellationToken token = default)
    {
        var actorId = Guid.Parse(routeData!.Values["actorId"]!.ToString()!);
        var blog = await _blogRepository.GetByIdAsync(actorId, token);
        var actor = blog?.ToActor();

        if (actor == null)
        {
            _logger.LogInformation("Actor {Id} not found.", actorId);
            return NotFound();
        }

        _logger.LogInformation("Actor {Id} found.", actorId);
        return ActivityStreams(actor);
    }
}
