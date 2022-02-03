using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace Tumblepub.ActivityPub.Endpoints;

public record EndpointPointer(HttpMethod Method, string Path, Type EndpointType);

public interface IEndpoint
{
    Task<IActionResult> InvokeAsync(HttpContext context, RouteData? routeData, CancellationToken token = default);
}

public abstract class Endpoint : IEndpoint
{
    public abstract Task<IActionResult> InvokeAsync(HttpContext context, RouteData? routeData, CancellationToken token = default);

    public virtual OkResult Ok() => new();

    public virtual OkObjectResult Ok(object value) => new(value);

    public virtual NotFoundResult NotFound() => new();

    public virtual NotFoundObjectResult NotFound(object value) => new(value);
}
