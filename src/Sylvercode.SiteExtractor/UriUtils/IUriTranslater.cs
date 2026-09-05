namespace Sylvercode.SiteExtractor.UriUtils;

/// <summary>Defines the contract for translating a <see cref="Uri"/> from one form to another during extraction</summary>
public interface IUriTranslater
{
    /// <summary>Translates <paramref name="uri"/> and returns the transformed result</summary>
    /// <param name="uri">The source URI to translate</param>
    /// <returns>The translated URI</returns>
    Uri Translate(Uri uri);
}
