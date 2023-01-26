using HandlebarsDotNet;
using System.Reflection;
using AngleSharp;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace Tumblepub.Themes;

public static class DefaultTheme
{
    public static readonly Lazy<HandlebarsTemplate<object, object>> Template = new(() => Handlebars.Compile(ReadFromResource()));

    private static string ReadFromResource()
    {
        var assembly = typeof(DefaultTheme).GetTypeInfo().Assembly;
        var resourceName = assembly.GetManifestResourceNames()
            .First(r => r.EndsWith("DefaultTheme.hbs", StringComparison.CurrentCultureIgnoreCase));

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new InvalidOperationException("Could not load default theme as embedded resource.");
        }

        var parser = new HtmlParser();
        var document = parser.ParseDocument(stream);
        var body = document.Body;
        
        // inject Blazor stuff into the document
        var blazorCustomElementScript = document.CreateElement("script");
        blazorCustomElementScript.SetAttribute("src", "_content/Microsoft.AspNetCore.Components.CustomElements/BlazorCustomElements.js");
        body.AppendChild(blazorCustomElementScript);
        
        var blazorScript = document.CreateElement("script");
        blazorScript.SetAttribute("src", "_framework/blazor.webassembly.js");
        body.AppendChild(blazorScript);
        
        return document.DocumentElement.ToHtml();
    }
}
