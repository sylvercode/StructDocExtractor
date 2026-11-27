using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.Manifest;
using Sylvercode.SiteExtractor.Store;

namespace Sylvercode.SiteExtractor.Tests;

/// <summary>Tests for file-system output-path protection in <see cref="DirectoryDataStore"/>.</summary>
public class DirectoryDataStoreTests
{
    /// <summary>Verifies that the reserved manifest path is rejected before an existing file is opened.</summary>
    [Fact]
    public void GetStream_ReservedManifestPath_PreservesExistingManifest()
    {
        string root = Path.Combine(
            Path.GetTempPath(),
            "DDBMarkdown-data-store-tests",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        string manifestPath = Path.Combine(
            root,
            ExtractionManifestValidator.ManifestFileName);
        string original = "existing manifest";
        File.WriteAllText(manifestPath, original);
        try
        {
            SiteExtractorOptions options = new()
            {
                OutputDirectory = root,
            };
            DirectoryDataStore store = new(Options.Create(options));

            Assert.Throws<ArgumentException>(
                () => store.GetStream(
                    new Uri(
                        ExtractionManifestValidator.ManifestFileName,
                        UriKind.Relative)));

            Assert.Equal(original, File.ReadAllText(manifestPath));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    /// <summary>Verifies that only the root-level reserved path is rejected.</summary>
    [Fact]
    public void GetStream_NestedManifestName_IsAllowed()
    {
        string root = Path.Combine(
            Path.GetTempPath(),
            "DDBMarkdown-data-store-tests",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            DirectoryDataStore store = new(Options.Create(
                new SiteExtractorOptions { OutputDirectory = root }));
            using Stream stream = store.GetStream(
                new Uri(
                    $"nested/{ExtractionManifestValidator.ManifestFileName}",
                    UriKind.Relative));

            Assert.True(stream.CanWrite);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }
}
