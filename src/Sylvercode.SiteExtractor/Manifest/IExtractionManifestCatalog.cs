using Sylvercode.SiteExtractor.Resources;

namespace Sylvercode.SiteExtractor.Manifest;

/// <summary>Coordinates manifest validation, retention, and persistence for an extraction.</summary>
public interface IExtractionManifestCatalog
{
    /// <summary>Gets loaded linked manifests by canonical output root.</summary>
    IReadOnlyDictionary<string, ExtractionManifest> LinkedManifests { get; }

    /// <summary>Validates the configured roots and loads linked manifests before crawling starts.</summary>
    void BeginExtraction(Uri seedUri);

    /// <summary>Builds and persists the current manifest after every resource has completed.</summary>
    void CompleteExtraction(IReadOnlyResourceRepository resources);
}
