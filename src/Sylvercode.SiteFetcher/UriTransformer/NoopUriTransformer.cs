namespace Sylvercode.SiteFetcher.UriTransformer;

public class NoopUriTransformer : IUriTransformer
{
    public static NoopUriTransformer Default { get; } = new NoopUriTransformer();
    public Uri Transform(Uri uri) => uri;
}
