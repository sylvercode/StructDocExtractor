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

    [Fact]
    public void GetStream_DoubleEncodedReservedManifest_IsNotRedirectedToManifest()
    {
        string root = NewRoot();
        string manifestPath = Path.Combine(
            root,
            ExtractionManifestValidator.ManifestFileName);
        string original = "existing manifest";
        File.WriteAllText(manifestPath, original);
        try
        {
            DirectoryDataStore store = NewStore(root);
            using Stream stream = store.GetStream(
                new Uri(
                    "%2565xtraction-manifest.json",
                    UriKind.Relative));

            Assert.Equal(original, File.ReadAllText(manifestPath));
            Assert.True(File.Exists(Path.Combine(
                root,
                "%65xtraction-manifest.json")));
            Assert.True(stream.CanWrite);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void GetStream_EncodedSpace_WritesDecodedFileName()
    {
        string root = NewRoot();
        try
        {
            DirectoryDataStore store = NewStore(root);
            using Stream stream = store.GetStream(
                new Uri("report%20final.md", UriKind.Relative));

            Assert.True(File.Exists(Path.Combine(root, "report final.md")));
            Assert.False(File.Exists(Path.Combine(root, "report%20final.md")));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void GetStream_EncodedHash_WritesDecodedFileName()
    {
        string root = NewRoot();
        try
        {
            DirectoryDataStore store = NewStore(root);
            using Stream stream = store.GetStream(
                new Uri("report%23a.md", UriKind.Relative));

            Assert.True(File.Exists(Path.Combine(root, "report#a.md")));
            Assert.True(stream.CanWrite);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void GetStream_FileNameContainingDoubleDots_IsAllowed()
    {
        string root = NewRoot();
        try
        {
            DirectoryDataStore store = NewStore(root);
            using (StreamWriter writer = store.GetStreamWriter(
                new Uri("file..md", UriKind.Relative)))
            {
                writer.Write("content");
            }

            Assert.Equal("content", File.ReadAllText(Path.Combine(root, "file..md")));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Theory]
    [InlineData("./extraction-manifest.json")]
    [InlineData("nested/../extraction-manifest.json")]
    [InlineData("%2e/extraction-manifest.json")]
    [InlineData("nested/%2e%2e/extraction-manifest.json")]
    public void GetStream_DotSegmentManifestAlias_IsRejected(string outputUri)
    {
        string root = NewRoot();
        string manifestPath = Path.Combine(
            root,
            ExtractionManifestValidator.ManifestFileName);
        string original = "existing manifest";
        File.WriteAllText(manifestPath, original);
        try
        {
            DirectoryDataStore store = NewStore(root);

            Assert.Throws<ArgumentException>(
                () => store.GetStream(
                    new Uri(outputUri, UriKind.Relative)));
            Assert.Equal(original, File.ReadAllText(manifestPath));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static string NewRoot()
    {
        string root = Path.Combine(
            Path.GetTempPath(),
            "DDBMarkdown-data-store-tests",
            Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        return root;
    }

    private static DirectoryDataStore NewStore(string root)
        => new(Options.Create(
            new SiteExtractorOptions { OutputDirectory = root }));
}
