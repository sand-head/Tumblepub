using Microsoft.AspNetCore.Mvc;
using Tumblepub.ActivityPub.Filters;

namespace Tumblepub.ActivityPub.Controllers;

[ApiController]
[Route("[controller]")]
[ServiceFilter(typeof(JsonLDAsyncActionFilter))]
public class UsersController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUser()
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    public async Task<IActionResult> GetUserActivity()
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    public async Task<IActionResult> GetUserPost()
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    public async Task<IActionResult> GetUserFollowers()
    {
        throw new NotImplementedException();
    }

    [HttpGet]
    public async Task<IActionResult> GetUserOutbox()
    {
        throw new NotImplementedException();
    }

    [HttpPost]
    public async Task<IActionResult> PostUserInbox()
    {
        throw new NotImplementedException();
    }
}
