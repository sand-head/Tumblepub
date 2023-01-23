using Microsoft.Extensions.DependencyInjection;
using Tumblepub.ActivityPub.Endpoints;
using Tumblepub.ActivityPub.Services;

namespace Tumblepub.ActivityPub.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddActivityPub(this IServiceCollection services)
    {
        return services
            .AddHostedService<ActivityDeliveryService>()
            .AddDefaultEndpoints();
    }

    internal static IServiceCollection AddDefaultEndpoints(this IServiceCollection services)
    {
        return services
            .AddEndpoint<GetActorActivityEndpoint>(HttpMethod.Get, string.Format(ActivityPubConstants.ActorActivityRoute, "{actorId}", "{activityId}"))
            .AddEndpoint<GetActorEndpoint>(HttpMethod.Get, string.Format(ActivityPubConstants.ActorRoute, "{actorId}"))
            .AddEndpoint<GetActorFollowersEndpoint>(HttpMethod.Get, string.Format(ActivityPubConstants.ActorFollowersRoute, "{actorId}"))
            .AddEndpoint<GetActorObjectEndpoint>(HttpMethod.Get, string.Format(ActivityPubConstants.ActorObjectRoute, "{actorId}", "{objectId}"))
            .AddEndpoint<GetActorOutboxEndpoint>(HttpMethod.Get, string.Format(ActivityPubConstants.ActorOutboxRoute, "{actorId}"))
            .AddEndpoint<PostActorInboxEndpoint>(HttpMethod.Post, string.Format(ActivityPubConstants.ActorInboxRoute, "{actorId}"));
    }

    public static IServiceCollection AddEndpoint<TEndpoint>(this IServiceCollection services, HttpMethod method, string path)
        where TEndpoint : class, IEndpoint
    {
        return services
            .AddSingleton(new EndpointPointer(method, path, typeof(TEndpoint)))
            .AddTransient<TEndpoint>();
    }
}
