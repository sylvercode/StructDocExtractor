using Sylvercode.SiteExtractor.Manifest;

namespace Sylvercode.SiteExtractor.Store;

/// <summary>Loads and writes extraction manifests independently of extracted output streams.</summary>
public interface IExtractionManifestStore
{
    /// <summary>Loads the manifest directly below <paramref name="outputRoot"/>.</summary>
    ExtractionManifest Load(string outputRoot);

    /// <summary>Writes a complete, validated manifest below <paramref name="outputRoot"/>.</summary>
    void Write(string outputRoot, ExtractionManifest manifest);
}
