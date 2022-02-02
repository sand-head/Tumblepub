using Microsoft.Extensions.DependencyInjection;
using Tumblepub.ActivityPub.Filters;

namespace Tumblepub.ActivityPub.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddActivityPub<TAPUserService>(this IServiceCollection services)
        where TAPUserService : class, IActivityPubService
    {
        return services
            .AddScoped<IActivityPubService, TAPUserService>()
            .AddScoped<JsonLDAsyncActionFilter>();
    }
}
