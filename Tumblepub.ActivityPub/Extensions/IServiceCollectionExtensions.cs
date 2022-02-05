using Microsoft.Extensions.DependencyInjection;
using Tumblepub.ActivityPub.Endpoints;
using Tumblepub.ActivityPub.Services;

namespace Tumblepub.ActivityPub.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddActivityPub<TAPUserService>(this IServiceCollection services, Action<ActivityPubOptions>? configureOptions = null)
        where TAPUserService : class, IActivityPubService
    {
        var options = new ActivityPubOptions();

        configureOptions?.Invoke(options);

        return services
            .AddSingleton(options)
            .AddScoped<IActivityPubService, TAPUserService>()
            .AddHostedService<ActivityDeliveryService>()
            .AddDefaultEndpoints(options);
    }

    internal static IServiceCollection AddDefaultEndpoints(this IServiceCollection services, ActivityPubOptions options)
    {
        return services
            .AddEndpoint<GetActorEndpoint>(HttpMethod.Get, options.ActorRouteTemplate)
            .AddEndpoint<GetActorFollowersEndpoint>(HttpMethod.Get, options.ActorFollowersRouteTemplate)
            .AddEndpoint<PostActorInboxEndpoint>(HttpMethod.Post, options.ActorInboxRouteTemplate);
    }

    public static IServiceCollection AddEndpoint<TEndpoint>(this IServiceCollection services, HttpMethod method, string path)
        where TEndpoint : class, IEndpoint
    {
        return services
            .AddSingleton(new EndpointPointer(method, path, typeof(TEndpoint)))
            .AddTransient<TEndpoint>();
    }
}
