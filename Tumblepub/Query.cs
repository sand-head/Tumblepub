using HotChocolate.AspNetCore.Authorization;
using System.Security.Claims;
using Tumblepub.Application.Interfaces;
using Tumblepub.Application.Models;

namespace Tumblepub;

public class Query
{
    // todo: control this in build
    public string ApiVersion() => "0.2";

    [Authorize]
    public async Task<User> GetCurrentUser(ClaimsPrincipal claimsPrincipal, [Service] IRepository<User, UserId> userRepository)
    {
        var userIdClaimValue = claimsPrincipal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var userId = new UserId(Guid.Parse(userIdClaimValue));

        var userDto = await userRepository.GetByIdAsync(userId);
        return userDto!;
    }
}
