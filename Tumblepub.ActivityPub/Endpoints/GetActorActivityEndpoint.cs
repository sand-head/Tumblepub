using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tumblepub.ActivityPub.Endpoints;

public sealed class GetActorActivityEndpoint : Endpoint
{
    public override Task<IActionResult> InvokeAsync(RouteData? routeData, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
