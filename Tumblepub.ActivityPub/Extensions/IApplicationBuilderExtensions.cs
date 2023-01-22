using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using System.Net;
using Tumblepub.ActivityPub.Endpoints;
using Tumblepub.ActivityPub.Webfinger;
using Tumblepub.Application.Aggregates;
using Tumblepub.Application.Blog.Queries;
using Tumblepub.Application.Interfaces;

namespace Tumblepub.ActivityPub.Extensions;

public static class IApplicationBuilderExtensions
{
    public static IApplicationBuilder UseActivityPub(this IApplicationBuilder app)
    {
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
                                endpoint.Context = context;
                                var routeData = context.GetRouteData();
                                var result = await endpoint.InvokeAsync(routeData, context.RequestAborted);

                                if (result != null)
                                {
                                    await result.ExecuteResultAsync(new ActionContext { HttpContext = context });
                                }
                            }
                        });
                }

                app.UseRouter(builder.Build());
            });

        // map endpoints not locked behind ActivityPub headers
        var builder = new RouteBuilder(app);

        builder
            .MapWellKnownWebfingerEndpoint()
            .MapWellKnownNodeInfoEndpoint();

        builder.MapGet(ActivityPubConstants.ActorRoute, async context =>
        {
            using var scope = context.RequestServices.CreateScope();
            var queryHandler = scope.ServiceProvider.GetRequiredService<IQueryHandler<GetBlogByIdQuery, Blog?>>();

            // get blog ID from route values
            var routeValues = context.GetRouteData().Values;
            var blogId = Guid.Parse(routeValues["0"]!.ToString()!);

            var query = new GetBlogByIdQuery(blogId);
            var blog = await queryHandler.Handle(query, context.RequestAborted);
            if (blog == null)
            {
                return;
            }

            // redirect to the generated URL
            context.Response.Redirect($"/@{blog.Name}");
        });

        return app.UseRouter(builder.Build());
    }

    private static IRouteBuilder MapWellKnownWebfingerEndpoint(this IRouteBuilder builder)
    {
        return builder.MapGet("/.well-known/webfinger", async context => {
            if (!context.Request.Query.TryGetValue("resource", out var resourceValues) || resourceValues.Count != 1)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            // get the username and domain out of the resource string
            var resource = resourceValues.ToString()
                .Replace("acct:", string.Empty)
                .Split('@');
            var name = resource[0];
            var domain = resource[1];

            // check to make sure that the domain is the same one the request has been made to
            var currentDomain = context.Request.Host.Host;
            if (currentDomain.StartsWith("www."))
            {
                currentDomain = currentDomain["www.".Length..];
            }
            if (currentDomain != domain)
            {
                return;
            }

            // get the actor by their name
            using var scope = context.RequestServices.CreateScope();
            var queryHandler = scope.ServiceProvider.GetRequiredService<IQueryHandler<GetBlogByNameQuery, Blog?>>();
            
            var query = new GetBlogByNameQuery(name);
            var blog = await queryHandler.Handle(query, context.RequestAborted);
            if (blog == null)
            {
                return;
            }

            var domainWithSchemeAndPort = $"{context.Request.Scheme}://{context.Request.Host.Value}";
            await context.Response.WriteAsJsonAsync(new WebfingerActor
            {
                Subject = $"acct:{name}@{domain}",
                Links = new()
                {
                    new WebfingerLink
                    {
                        Rel = "self",
                        Type = "application/activity+json",
                        Href = domainWithSchemeAndPort + blog.Id
                    }
                }
            });
        });
    }

    private static IRouteBuilder MapWellKnownNodeInfoEndpoint(this IRouteBuilder builder)
    {
        return builder.MapGet("/.well-known/nodeinfo", async context => {
            throw new NotImplementedException();
        });
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
        if (!header.SelectMany(h => h.Split(',')).Any(v => IsActivityPubHeaderValue(v.Trim())))
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
