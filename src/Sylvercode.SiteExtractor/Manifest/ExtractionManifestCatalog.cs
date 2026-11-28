namespace Sylvercode.SiteExtractor.Manifest;

/// <summary>Retains loaded manifests for linked extraction roots.</summary>
public sealed class ExtractionManifestCatalog : IExtractionManifestCatalog
{
    private IReadOnlyDictionary<string, ExtractionManifest> _linkedManifests =
        new Dictionary<string, ExtractionManifest>(StringComparer.Ordinal);

    /// <inheritdoc/>
    public IReadOnlyDictionary<string, ExtractionManifest> LinkedManifests => _linkedManifests;

    /// <inheritdoc/>
    public void SetLinkedManifests(IReadOnlyDictionary<string, ExtractionManifest> manifests)
        => _linkedManifests = new Dictionary<string, ExtractionManifest>(
            manifests,
            StringComparer.Ordinal);
}
