using Microsoft.Extensions.DependencyInjection;

namespace Tumblepub.Application.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}