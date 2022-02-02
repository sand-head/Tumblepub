using Microsoft.AspNetCore.Mvc;
using Tumblepub.ActivityPub.Filters;

namespace Tumblepub.ActivityPub.Controllers;

[ApiController]
[Route("[controller]")]
[ServiceFilter(typeof(JsonLDAsyncActionFilter))]
public class UsersController : ControllerBase
{
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser(Guid userId, [FromServices] IActivityPubService activityPubService)
    {
        var user = await activityPubService.GetUser(userId);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpGet("{userId}/followers")]
    public async Task<IActionResult> GetUserFollowers(Guid userId)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{userId}/outbox")]
    public async Task<IActionResult> GetUserOutbox(Guid userId)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{userId}/activity/{activityId}")]
    public async Task<IActionResult> GetUserActivity(Guid userId, Guid activityId)
    {
        throw new NotImplementedException();
    }

    [HttpGet("{userId}/post/{postId}")]
    public async Task<IActionResult> GetUserPost(Guid userId, Guid postId)
    {
        throw new NotImplementedException();
    }

    [HttpPost("{userId}/inbox")]
    public async Task<IActionResult> PostUserInbox(Guid userId)
    {
        throw new NotImplementedException();
    }
}
