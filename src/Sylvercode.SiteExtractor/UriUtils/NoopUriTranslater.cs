namespace Sylvercode.SiteExtractor.UriUtils;

/// <summary>Identity implementation of <see cref="IUriTranslater"/> that returns its input URI unchanged</summary>
public class NoopUriTranslater : IUriTranslater
{
    /// <summary>Gets the shared singleton instance of <see cref="NoopUriTranslater"/></summary>
    public static NoopUriTranslater Default { get; } = new NoopUriTranslater();

    /// <inheritdoc/>
    public Uri Translate(Uri uri)
        => uri;
}
