using Marten;
using Marten.Events.Projections;
using Marten.Services.Json;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Tumblepub.Application.Interfaces;
using Tumblepub.Application.Models;
using Tumblepub.Infrastructure.Projections;
using Tumblepub.Infrastructure.Repositories;
using Weasel.Core;

namespace Tumblepub.Infrastructure.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString, bool isDevelopment = false)
    {
        services.AddMarten(options =>
        {
            // configuration
            options.Connection(connectionString);
            
            options.UseDefaultSerialization(serializerType: SerializerType.SystemTextJson);

            options.AutoCreateSchemaObjects = isDevelopment
                ? AutoCreate.All
                : AutoCreate.CreateOrUpdate;
            options.Events.DatabaseSchemaName = "events";
            options.CreateDatabasesForTenants(configure =>
            {
                configure.ForTenant()
                    .WithOwner("postgres")
                    .WithEncoding("UTF-8")
                    .ConnectionLimit(-1)
                    .OnDatabaseCreated(_ =>
                    {
                        Log.Information("Created database!");
                    });
            });
            
            // document database junk
            options.Schema.For<Blog>().ForeignKey<User>(b => b.UserId);
            options.Schema.For<BlogActivity>().ForeignKey<Blog>(b => b.BlogId);
            options.Schema.For<Post>().ForeignKey<Blog>(p => p.BlogId);

            // actual event sourcing junk
            options.Projections.Add<UserProjection>();
            options.Projections.Add<BlogProjection>();
            options.Projections.Add<PostProjection>();

            options.Projections.Add<BlogActivityProjection>(ProjectionLifecycle.Inline);
        });
        
        return services
            .AddScoped<IRepository<User>, MartenRepository<User>>()
            .AddScoped<IRepository<Blog>, MartenRepository<Blog>>()
            .AddScoped<IRepository<Post>, MartenRepository<Post>>()
            .AddScoped<IRepository<BlogActivity>, MartenRepository<BlogActivity>>()
            .AddScoped<IBlogActivityRepository, BlogActivityRepository>();
    }
}