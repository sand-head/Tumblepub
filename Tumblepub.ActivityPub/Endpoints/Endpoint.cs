using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tumblepub.ActivityPub.ActivityStreams;
using Tumblepub.ActivityPub.Converters;

namespace Tumblepub.ActivityPub.Endpoints;

public record EndpointPointer(HttpMethod Method, string Path, Type EndpointType);

public interface IEndpoint
{
    HttpContext Context { get; set; }
    Task<IActionResult> InvokeAsync(RouteData? routeData, CancellationToken token = default);
}

public abstract class Endpoint : IEndpoint
{
    public HttpContext Context { get; set; } = default!;

    public abstract Task<IActionResult> InvokeAsync(RouteData? routeData, CancellationToken token = default);

    public virtual NotFoundResult NotFound() => new();
    public virtual NotFoundObjectResult NotFound(object value) => new(value);

    public virtual ContentResult ActivityStreams<TValue>(TValue value)
        where TValue : ActivityStreamsValue
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        options.Converters.Add(new RelativeToAbsoluteUriConverter(Context));
        options.Converters.Add(new ActivityStreamsValueConverter());

        return new ContentResult
        {
            Content = JsonSerializer.Serialize(value, options),
            ContentType = ActivityPubConstants.ContentType,
            StatusCode = (int)HttpStatusCode.OK
        };
    }
}
