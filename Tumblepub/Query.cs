using HotChocolate.AspNetCore.Authorization;
using System.Security.Claims;
using Tumblepub.Database.Models;
using Tumblepub.Database.Repositories;

namespace Tumblepub;

public class Query
{
    // todo: control this in build
    public string ApiVersion() => "0.2";

    [Authorize]
    public async Task<UserDto> GetCurrentUser(ClaimsPrincipal claimsPrincipal, [Service] IUserDtoRepository userDtoRepository)
    {
        var userIdClaimValue = claimsPrincipal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var userId = Guid.Parse(userIdClaimValue);

        var userDto = await userDtoRepository.GetByIdAsync(userId);
        return userDto!;
    }
}
