using Sylvercode.SiteExtractor.Manifest;

namespace Sylvercode.SiteExtractor.Store;

/// <summary>In-memory extraction-manifest store intended for tests and transient extraction pipelines.</summary>
public sealed class MemoryExtractionManifestStore : IExtractionManifestStore
{
    private readonly Dictionary<string, ExtractionManifest> _manifests = new(StringComparer.Ordinal);

    /// <summary>Gets the manifests currently stored by canonical output root.</summary>
    public IReadOnlyDictionary<string, ExtractionManifest> Manifests => _manifests.AsReadOnly();

    /// <inheritdoc/>
    public ExtractionManifest Load(string outputRoot)
    {
        string root = ExtractionManifestValidator.GetCanonicalOutputRoot(outputRoot, "outputDirectory");
        if (!_manifests.TryGetValue(root, out ExtractionManifest? manifest))
            throw ExtractionManifestErrors.InvalidManifest(root, "$", $"Required manifest '{ExtractionManifestValidator.ManifestFileName}' is missing.");

        return ExtractionManifestValidator.ValidateAndNormalize(manifest, root);
    }

    /// <inheritdoc/>
    public void Write(string outputRoot, ExtractionManifest manifest)
    {
        string root = ExtractionManifestValidator.GetCanonicalOutputRoot(outputRoot, "outputDirectory");
        _manifests[root] = ExtractionManifestValidator.ValidateAndNormalize(manifest, root);
    }
}
