using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Tumblepub.Projections;
using Tumblepub.Services;

namespace Tumblepub;

public class Query
{
    // todo: control this in build
    public string ApiVersion() => "0.2";

    [Authorize]
    public async Task<UserDto> GetCurrentUser(ClaimsPrincipal claimsPrincipal, [Service] IUserDtoService userDtoService)
    {
        var userIdClaimValue = claimsPrincipal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var userId = Guid.Parse(userIdClaimValue);

        var userDto = await userDtoService.GetByIdAsync(userId);
        return userDto!;
    }
}
