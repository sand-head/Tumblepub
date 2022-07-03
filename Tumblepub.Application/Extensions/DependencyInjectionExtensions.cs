using Microsoft.Extensions.DependencyInjection;
using Tumblepub.Application.Interfaces;

namespace Tumblepub.Application.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // register all command/query handlers
        return services
            .Scan(scan => scan
                .FromAssembliesOf(typeof(Models.Blog))
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                    .AsImplementedInterfaces()
                    .WithScopedLifetime());
    }
}