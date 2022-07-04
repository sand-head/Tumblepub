using HotChocolate.AspNetCore.Authorization;
using System.Security.Claims;
using Tumblepub.Application.Aggregates;
using Tumblepub.Application.Interfaces;

namespace Tumblepub;

public class Query
{
    // todo: control this in build
    public string ApiVersion() => "0.2";

    [Authorize]
    public async Task<User> GetCurrentUser(ClaimsPrincipal claimsPrincipal, [Service] IRepository<User, Guid> userRepository)
    {
        var userIdClaimValue = claimsPrincipal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var userId = Guid.Parse(userIdClaimValue);

        var userDto = await userRepository.GetByIdAsync(userId);
        return userDto!;
    }
}
