using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

namespace Tumblepub.ActivityPub.Endpoints;

public sealed class UserEndpoint : Endpoint
{
    private readonly IActivityPubService _activityPubService;
    private readonly ILogger<UserEndpoint> _logger;

    public UserEndpoint(IActivityPubService activityPubService, ILogger<UserEndpoint> logger)
    {
        _activityPubService = activityPubService ?? throw new ArgumentNullException(nameof(activityPubService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<IActionResult> InvokeAsync(HttpContext context, RouteData? routeData, CancellationToken token = default)
    {
        var userId = Guid.Parse(routeData!.Values["userId"]!.ToString()!);
        var user = await _activityPubService.GetUser(userId, token);

        if (user == null)
        {
            _logger.LogInformation("User {Id} not found.", userId);
            return NotFound();
        }

        _logger.LogInformation("User {Id} found.", userId);
        return Ok(user);
    }
}
