namespace Sylvercode.SiteFetcher.Resource;

public class NoopUriTransformer : IUriTransformer
{
    public static NoopUriTransformer Default { get; } = new NoopUriTransformer();
    public Uri Transform(Uri uri) => uri;
}
