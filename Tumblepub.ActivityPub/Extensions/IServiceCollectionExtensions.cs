using Microsoft.Extensions.DependencyInjection;
using Tumblepub.ActivityPub.Endpoints;

namespace Tumblepub.ActivityPub.Extensions;

public static class IServiceCollectionExtensions
{
    public static IServiceCollection AddActivityPub<TAPUserService>(this IServiceCollection services)
        where TAPUserService : class, IActivityPubService
    {
        return services
            .AddScoped<IActivityPubService, TAPUserService>()
            .AddEndpoints();
    }

    public static IServiceCollection AddEndpoints(this IServiceCollection services)
    {
        return services
            .AddEndpoint<UserEndpoint>(HttpMethod.Get, "/users/{userId}");
    }

    public static IServiceCollection AddEndpoint<TEndpoint>(this IServiceCollection services, HttpMethod method, string path)
        where TEndpoint : class, IEndpoint
    {
        return services
            .AddSingleton(new EndpointPointer(method, path, typeof(TEndpoint)))
            .AddTransient<TEndpoint>();
    }
}
