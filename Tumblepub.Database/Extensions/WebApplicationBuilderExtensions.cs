using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;
using Tumblepub.Database.Projections;
using Weasel.Postgresql;

namespace Tumblepub.Database.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddEventSourcing(this WebApplicationBuilder builder, string connectionString)
    {
        builder.Services.AddMarten(options =>
        {
            options.Connection(connectionString);

            options.AutoCreateSchemaObjects = builder.Environment.IsDevelopment()
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

            options.Projections.SelfAggregate<User>();
        });

        return builder;
    }
}
