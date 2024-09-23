namespace Sylvercode.SiteExtractor.UriUtils;

public static class UriExtentions
{
    public static Tuple<Uri, string> GetUriAndFragment(this Uri uri)
    {
        string fragment = uri.IsAbsoluteUri ? uri.Fragment : string.Empty;
        Uri uriWithoutFragment = new(uri.GetComponents(UriComponents.AbsoluteUri & ~UriComponents.Fragment, UriFormat.Unescaped));
        return new Tuple<Uri, string>(uriWithoutFragment, fragment);
    }
}
