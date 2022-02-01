using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;

namespace Tumblepub.ActivityPub.Filters;

internal class JsonLDAsyncActionFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        StringValues header;

        // get the appropriate header based on request method
        if (context.HttpContext.Request.Method.Equals("POST", StringComparison.InvariantCultureIgnoreCase))
        {
            header = context.HttpContext.Request.Headers["Content-Type"];
        }
        else if (context.HttpContext.Request.Method.Equals("GET", StringComparison.InvariantCultureIgnoreCase))
        {
            header = context.HttpContext.Request.Headers["Accept"];
        }
        else
        {
            // we don't need to filter anything that isn't POST or GET
            await next();
            return;
        }

        // if it's an ActivityPub header value, don't filter
        if (header.Any(v => IsActivityPubHeaderValue(v)))
        {
            await next();
        }
        else
        {
            context.Result = new BadRequestObjectResult("Not a valid ActivityPub request!");
        }
    }

    private static bool IsActivityPubHeaderValue(string value)
    {
        return value == "application/activity+json"
            || value == "application/ld+json; profile=\"https://www.w3.org/ns/activitystreams\"";
    }
}
