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

        Handlebars.RegisterHelper("not", (writer, context, args) =>
        {
            args.RequireLength("not", 1);

            var value = HandlebarsUtils.IsFalsyOrEmpty(args[0]);

            writer.WriteSafeString(value);
        });

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

    private static void RegisterLogicalOperator(string name, Func<bool, bool, bool> func) =>
        RegisterBinaryOperator(name,
            left => HandlebarsUtils.IsTruthyOrNonEmpty(left),
            right => HandlebarsUtils.IsTruthyOrNonEmpty(right),
            func);

    private static void RegisterComparisonOperator(string name, Func<object, object, bool> func) =>
        RegisterBinaryOperator(name, left => left, right => right, func);

    private static void RegisterBinaryOperator<TLeft, TRight, TResult>(string name, Func<object, TLeft> leftFunc, Func<object, TRight> rightFunc, Func<TLeft, TRight, TResult> resultFunc)
    {
        Handlebars.RegisterHelper(name, (writer, context, args) =>
        {
            args.RequireLength(name, 2);

            var left = leftFunc(args[0]);
            var right = rightFunc(args[1]);

            writer.WriteSafeString(resultFunc(left, right));
        });
    }

    private static void RequireLength(this Arguments args, string name, int length)
    {
        if (args.Length != length)
        {
            throw new HandlebarsException($"Helper \"{name}\" helper requires {length} argument(s)");
        }
    }
}
