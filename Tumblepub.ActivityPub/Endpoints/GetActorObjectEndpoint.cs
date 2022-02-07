using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tumblepub.ActivityPub.Endpoints;

public sealed class GetActorObjectEndpoint : Endpoint
{
    public override Task<IActionResult> InvokeAsync(HttpContext context, RouteData? routeData, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
