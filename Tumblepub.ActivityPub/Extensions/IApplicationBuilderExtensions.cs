using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Tumblepub.ActivityPub.Endpoints;

namespace Tumblepub.ActivityPub.Extensions;

public static class IApplicationBuilderExtensions
{
    public static IApplicationBuilder UseActivityPub(this IApplicationBuilder app)
    {
        // todo: map well-known endpoints

        // map ActivityPub endpoints
        // we only want to route to these if the client supports some specific content types
        // otherwise, ideally, it'll fall back to other routes
        app.MapWhen(context => ClientSupportsActivityPub(context),
            app =>
            {
                var endpointPointers = app.ApplicationServices.GetServices<EndpointPointer>();
                var builder = new RouteBuilder(app);

                foreach (var endpointPointer in endpointPointers)
                {
                    builder.MapVerb(
                        verb: endpointPointer.Method.Method,
                        template: endpointPointer.Path,
                        async context =>
                        {
                            using var scope = context.RequestServices.CreateScope();
                            var possibleEndpoint = scope.ServiceProvider.GetRequiredService(endpointPointer.EndpointType);

                            if (possibleEndpoint is IEndpoint endpoint)
                            {
                                var routeData = context.GetRouteData();
                                var result = await endpoint.InvokeAsync(context, routeData, context.RequestAborted);

                                if (result != null)
                                {
                                    await result.ExecuteResultAsync(new ActionContext { HttpContext = context });
                                }
                            }
                        });
                }

                app.UseRouter(builder.Build());
            });

        return app;
    }

    private static bool ClientSupportsActivityPub(HttpContext context)
    {
        StringValues header;

        // get the appropriate header based on request method
        if (context.Request.Method.Equals("POST", StringComparison.InvariantCultureIgnoreCase))
        {
            header = context.Request.Headers["Content-Type"];
        }
        else if (context.Request.Method.Equals("GET", StringComparison.InvariantCultureIgnoreCase))
        {
            header = context.Request.Headers["Accept"];
        }
        else
        {
            // we don't need to filter anything that isn't POST or GET
            return false;
        }

        // if it's an ActivityPub header value, don't filter
        if (!header.Any(v => IsActivityPubHeaderValue(v)))
        {
            return false;
        }

        return true;
    }

    private static bool IsActivityPubHeaderValue(string value)
    {
        return value == "application/activity+json"
            || value == "application/ld+json; profile=\"https://www.w3.org/ns/activitystreams\"";
    }
}
