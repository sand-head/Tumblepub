using HandlebarsDotNet;
using Humanizer;
using System.Web;

namespace Tumblepub.Themes;

public static class Helpers
{
    public static void Register()
    {
        RegisterLogicalOperator("and", (left, right) => left && right);
        RegisterLogicalOperator("or", (left, right) => left || right);

        RegisterComparisonOperator("eq", (left, right) => left == right);
        RegisterComparisonOperator("ne", (left, right) => left != right);

        Handlebars.RegisterHelper("urlEncode", (writer, context, args) =>
        {
            args.RequireLength("urlEncode", 1);

            var value = HttpUtility.UrlEncode(args.At<string>(0));

            writer.WriteSafeString(value);
        });

        Handlebars.RegisterHelper("timeAgo", (writer, context, args) =>
        {
            args.RequireLength("timeAgo", 1);

            var date = args[0] switch
            {
                DateTime d => d,
                string s => DateTime.Parse(s),
                _ => throw new HandlebarsException("Helper \"timeAgo\" requires a valid date")
            };

            writer.WriteSafeString(date.Humanize());
        });

        Handlebars.RegisterHelper("formatDate", (writer, context, args) =>
        {
            args.RequireLength("formatDate", 2);

            var date = args[0] switch
            {
                DateTime d => d,
                string s => DateTime.Parse(s),
                _ => throw new HandlebarsException("Helper \"formatDate\" requires the first argument be a valid date")
            };

            writer.WriteSafeString(date.ToString(args.At<string>(1)));
        });
    }

    private static void RegisterLogicalOperator(string name, Func<bool, bool, bool> func)
    {
        Handlebars.RegisterHelper(name, (writer, options, context, args) =>
        {
            args.RequireLength(name, 2);

            var left = DetermineTruthiness(args[0]);
            var right = DetermineTruthiness(args[1]);

            if (func(left, right))
                options.Template(writer, context);
            else
                options.Inverse(writer, context);
        });
    }

    private static void RegisterComparisonOperator(string name, Func<object, object, bool> func)
    {
        Handlebars.RegisterHelper(name, (writer, options, context, args) =>
        {
            args.RequireLength(name, 2);

            var left = args[0];
            var right = args[1];

            if (func(left, right))
                options.Template(writer, context);
            else
                options.Inverse(writer, context);
        });
    }

    private static bool DetermineTruthiness(object? obj)
    {
        return obj switch
        {
            null => false,
            string s => s.Length > 0,
            int i => i != 0,
            // todo: add more types if necessary
            _ => true
        };
    }

    private static void RequireLength(this Arguments args, string name, int length)
    {
        if (args.Length != length)
        {
            throw new HandlebarsException($"Helper \"{name}\" helper requires {length} argument(s)");
        }
    }
}
