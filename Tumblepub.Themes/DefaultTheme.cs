using HandlebarsDotNet;
using System.Reflection;

namespace Tumblepub.Themes;

public static class DefaultTheme
{
    public static readonly Lazy<HandlebarsTemplate<object, object>> Template = new(() => Handlebars.Compile(ReadFromResource()));

    private static string ReadFromResource()
    {
        var assembly = typeof(DefaultTheme).GetTypeInfo().Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .First(r => r.EndsWith("index.hbs", StringComparison.CurrentCultureIgnoreCase));

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new InvalidOperationException("Could not load default theme as embedded resource.");
        }

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
