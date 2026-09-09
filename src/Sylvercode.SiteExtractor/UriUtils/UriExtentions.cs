namespace Sylvercode.SiteExtractor.UriUtils;

/// <summary>Provides extension methods for <see cref="Uri"/> covering fragment splitting and URI path normalisation</summary>
public static class UriExtentions
{
    /// <summary>Splits <paramref name="uri"/> into its fragment-stripped URI and the fragment string</summary>
    /// <param name="uri">The URI to decompose</param>
    /// <returns>A tuple containing the URI without its fragment and the fragment text (empty string if none)</returns>
    public static Tuple<Uri, string> GetUriAndFragment(this Uri uri)
    {
        string fragment = uri.IsAbsoluteUri ? uri.Fragment : string.Empty;
        Uri uriWithoutFragment = new(uri.GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Fragment, UriFormat.Unescaped));
        return new Tuple<Uri, string>(uriWithoutFragment, fragment);
    }

    /// <summary>Normalises <paramref name="uriPath"/> by replacing back-slashes with forward-slashes and ensuring a trailing slash</summary>
    /// <param name="uriPath">The raw URI path string to normalise</param>
    /// <returns>The normalised path string ending with <c>/</c></returns>
    public static string AsDirPath(this string uriPath)
    {
        string noBackSlashPath = uriPath.Replace('\\', '/');
        if (!noBackSlashPath.EndsWith('/'))
            noBackSlashPath += '/';
        return noBackSlashPath;
    }
}
