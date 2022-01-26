using Marten;
using Serilog;
using Weasel.Postgresql;

namespace Tumblepub.Extensions;

public static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder AddEventSourcing(this WebApplicationBuilder builder, string connectionString, Action<StoreOptions>? configure = null)
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

            configure?.Invoke(options);
        });

        return builder;
    }
}
