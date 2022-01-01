﻿using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Weasel.Postgresql;

namespace Tumblepub.EventSourcing.Extensions;

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
        });

        return builder;
    }
}