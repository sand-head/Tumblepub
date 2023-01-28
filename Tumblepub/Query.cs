using HotChocolate.AspNetCore.Authorization;
using System.Security.Claims;
using AutoMapper;
using Mediator;
using Tumblepub.Application.Aggregates;
using Tumblepub.Application.Interfaces;
using Tumblepub.Application.User.Queries;
using Tumblepub.Models;

namespace Tumblepub;

public class Query
{
    // todo: control this in build
    public string ApiVersion() => "0.2";

    [Authorize]
    public async Task<UserDto> GetCurrentUser(ClaimsPrincipal claimsPrincipal,
        [Service] IMapper mapper,
        [Service] IMediator mediator,
        CancellationToken token)
    {
        var userIdClaimValue = claimsPrincipal.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;
        var userId = Guid.Parse(userIdClaimValue);

        var query = new GetUserByIdQuery(userId);
        var user = await mediator.Send(query, token);
        return mapper.Map<UserDto>(user);
    }
}
