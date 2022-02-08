namespace Tumblepub.ActivityPub.Extensions;

internal static class UriExtensions
{
    public static Uri MakeAbsoluteUri(this Uri uri, Uri hostUri)
    {
        if (uri == null)
        {
            throw new ArgumentNullException(nameof(uri));
        }

        if (uri.IsAbsoluteUri)
        {
            return uri;
        }

        if (hostUri == null || !hostUri.IsAbsoluteUri)
        {
            throw new ArgumentException("Parameter must not be null and be an absolute URI.", nameof(hostUri));
        }

        return new Uri(hostUri, uri);
    }
}
