using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tumblepub.Infrastructure;

namespace Tumblepub.Extensions;

public static class IDataProtectionBuilderExtensions
{
    public static IDataProtectionBuilder PersistKeysToMarten(this IDataProtectionBuilder builder)
    {
        builder.Services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(services =>
        {
            return new ConfigureOptions<KeyManagementOptions>(options =>
            {
                options.XmlRepository = new MartenXmlRepository();
            });
        });

        return builder;
    }
}
