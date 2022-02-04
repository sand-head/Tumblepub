using Microsoft.Extensions.DependencyInjection;
using Tumblepub.ActivityPub.Endpoints;
using Tumblepub.ActivityPub.Services;

namespace Tumblepub.ActivityPub.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddActivityPub<TAPUserService>(this IServiceCollection services)
        where TAPUserService : class, IActivityPubService
    {
        return services
            .AddScoped<IActivityPubService, TAPUserService>()
            .AddHostedService<ActivityDeliveryService>()
            .AddEndpoints();
    }

    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        return services
            .AddEndpoint<GetActorEndpoint>(HttpMethod.Get, "/actors/{userId}")
            .AddEndpoint<GetActorFollowersEndpoint>(HttpMethod.Get, "/actors/{userId}/followers")
            .AddEndpoint<PostActorInboxEndpoint>(HttpMethod.Post, "/actors/{userId}/inbox");
    }

    public static IServiceCollection AddEndpoint<TEndpoint>(this IServiceCollection services, HttpMethod method, string path)
        where TEndpoint : class, IEndpoint
    {
        return services
            .AddSingleton(new EndpointPointer(method, path, typeof(TEndpoint)))
            .AddTransient<TEndpoint>();
    }
}
