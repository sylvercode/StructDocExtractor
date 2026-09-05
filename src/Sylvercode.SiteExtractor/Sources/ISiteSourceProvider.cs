namespace Sylvercode.SiteExtractor.Sources;

/// <summary>Defines the contract for providing the appropriate <see cref="ISiteSource"/> for a given URI.</summary>
public interface ISiteSourceProvider
{
    /// <summary>Returns the <see cref="ISiteSource"/> capable of serving the specified URI.</summary>
    /// <param name="uri">The URI for which a source is needed.</param>
    /// <returns>The <see cref="ISiteSource"/> that can provide content for the URI.</returns>
    ISiteSource GetSource(Uri uri);
}
