using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Tumblepub.ActivityPub.Endpoints;

public class GetActorOutboxEndpoint : Endpoint
{
    private readonly IActivityPubService _activityPubService;
    private readonly ILogger<GetActorOutboxEndpoint> _logger;

    public GetActorOutboxEndpoint(IActivityPubService activityPubService, ILogger<GetActorOutboxEndpoint> logger)
    {
        _activityPubService = activityPubService ?? throw new ArgumentNullException(nameof(activityPubService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<IActionResult> InvokeAsync(RouteData? routeData, CancellationToken token = default)
    {
        var userId = Guid.Parse(routeData!.Values["userId"]!.ToString()!);
        var pageQueryString = Context.Request.Query["page"].ToString();

        int? pageNumber = string.IsNullOrEmpty(pageQueryString) ? null : int.Parse(pageQueryString);
        var outbox = await _activityPubService.GetOutbox(userId, pageNumber, token);

        if (outbox == null)
        {
            _logger.LogInformation("Outbox for actor {Id} not found.", userId);
            return NotFound();
        }

        _logger.LogInformation("Outbox for actor {Id} found.", userId);
        return ActivityStreams(outbox);
    }
}
