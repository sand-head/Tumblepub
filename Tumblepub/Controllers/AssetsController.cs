using Jdenticon.AspNetCore;
using Microsoft.AspNetCore.Mvc;

namespace Tumblepub.Controllers;

[Route("api/[controller]")]
public class AssetsController : ControllerBase
{
    [HttpGet("avatar/{name}")]
    public async Task<IActionResult> GetUserAvatar(string name)
    {
        return IdenticonResult.FromValue(name, 128);
    }
}
