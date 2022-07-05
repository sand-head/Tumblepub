using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Tumblepub.Application.Aggregates;
using Tumblepub.Application.Interfaces;

namespace Tumblepub.ActivityPub.Endpoints;

public sealed class GetActorActivityEndpoint : Endpoint
{
    private readonly IReadOnlyRepository<BlogActivity, Guid> _blogActivityRepository;
    private readonly ILogger<GetActorActivityEndpoint> _logger;

    public GetActorActivityEndpoint(IReadOnlyRepository<BlogActivity, Guid> blogActivityRepository, ILogger<GetActorActivityEndpoint> logger)
    {
        _blogActivityRepository = blogActivityRepository;
        _logger = logger;
    }

    public override async Task<IActionResult> InvokeAsync(RouteData? routeData, CancellationToken token = default)
    {
        var blogId = Guid.Parse(routeData!.Values["userId"]!.ToString()!);
        var activityId = Guid.Parse(routeData!.Values["activityId"]!.ToString()!);
        
        var blogActivity = await _blogActivityRepository.GetByIdAsync(activityId, token);
        if (blogActivity == null || blogActivity.BlogId != actorId)
        {
            return null;
        }

        if (activity == null)
        {
            _logger.LogInformation("Activity {Id} not found.", activityId);
            return NotFound();
        }

        _logger.LogInformation("Activity {Id} found.", activityId);
        return ActivityStreams(activity);
    }
}
