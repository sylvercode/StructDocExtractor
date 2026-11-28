namespace Sylvercode.SiteExtractor.Manifest;

/// <summary>Retains validated manifests for linked extraction roots.</summary>
public interface IExtractionManifestCatalog
{
    /// <summary>Gets loaded linked manifests by canonical output root.</summary>
    IReadOnlyDictionary<string, ExtractionManifest> LinkedManifests { get; }

    /// <summary>Replaces the retained linked manifests with a validated set.</summary>
    void SetLinkedManifests(IReadOnlyDictionary<string, ExtractionManifest> manifests);
}
