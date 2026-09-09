namespace Sylvercode.SiteExtractor.Resources.Processors;

/// <summary>Contract for resolving the correct <see cref="IResourceProcessor"/> for a given resource URI.</summary>
public interface IResourceProcessorProvider
{
    /// <summary>Returns the <see cref="IResourceProcessor"/> registered for the given URI, or <see langword="null"/> if none matches.</summary>
    /// <param name="uri">The URI of the resource to process.</param>
    /// <returns>A matching <see cref="IResourceProcessor"/>, or <see langword="null"/> if no processor is registered for the URI.</returns>
    public IResourceProcessor? GetProcessor(Uri uri);
}
