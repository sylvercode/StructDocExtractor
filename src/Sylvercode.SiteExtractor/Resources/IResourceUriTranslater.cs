namespace Sylvercode.SiteExtractor.Resources;

/// <summary>Contract for translating a resource URI into its final output path.</summary>
public interface IResourceUriTranslater
{
    /// <summary>Translates <paramref name="uri"/> to the output URI based on the owning <paramref name="resource"/>.</summary>
    /// <param name="resource">The resource that owns the URI being translated.</param>
    /// <param name="uri">The source URI to translate, which may include a fragment.</param>
    /// <returns>The translated output URI.</returns>
    Uri Translate(Resource resource, Uri uri);
}
