using Microsoft.Extensions.DependencyInjection;
using Tumblepub.Application.Interfaces;

namespace Tumblepub.Application.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}