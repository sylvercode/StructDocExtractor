using Sylvercode.SiteExtractor.Manifest;
using Sylvercode.SiteExtractor.Store;

namespace Sylvercode.SiteExtractor.Tests;

public class ExtractionManifestStoreTests
{
    [Fact]
    public void WriteThenLoad_RoundTripsInStableOrder()
    {
        string root = NewTemporaryDirectory();
        try
        {
            DirectoryExtractionManifestStore store = new();
            store.Write(
                root,
                new ExtractionManifest(
                    1,
                    new Uri("https://example.com/docs/"),
                    [
                        new(new Uri("https://example.com/docs/z?q=1"), "z.md"),
                        new(new Uri("https://example.com/docs/a"), "nested/a.md"),
                    ]));

            ExtractionManifest loaded = store.Load(root);

            Assert.Collection(
                loaded.Entries,
                entry => Assert.Equal("nested/a.md", entry.OutputFile),
                entry => Assert.Equal("z.md", entry.OutputFile));
            string json = File.ReadAllText(Path.Combine(root, ExtractionManifestValidator.ManifestFileName));
            Assert.Contains("\"schemaVersion\": 1", json, StringComparison.Ordinal);
            Assert.True(json.IndexOf("nested/a.md", StringComparison.Ordinal)
                < json.IndexOf("\"z.md\"", StringComparison.Ordinal));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Theory]
    [InlineData("\"schemaVersion\":2", "schemaVersion")]
    [InlineData("\"unknownField\":true", "unknownField")]
    [InlineData("\"outputFile\":\"../escape.md\"", "outputFile")]
    [InlineData("\"outputFile\":\"extraction-manifest.json\"", "outputFile")]
    [InlineData("\"sourceUri\":\"relative\"", "sourceUri")]
    [InlineData("\"sourceUri\":\"https://example.com/docs/page#fragment\"", "sourceUri")]
    public void Load_InvalidManifest_ThrowsUsefulError(string replacement, string expectedField)
    {
        string root = NewTemporaryDirectory();
        try
        {
            string json = ValidJson();
            json = replacement.StartsWith("\"schemaVersion", StringComparison.Ordinal)
                ? json.Replace("\"schemaVersion\":1", replacement, StringComparison.Ordinal)
                : replacement.StartsWith("\"unknownField", StringComparison.Ordinal)
                    ? json.Replace("\"entries\":", replacement + ",\"entries\":", StringComparison.Ordinal)
                    : replacement.StartsWith("\"outputFile", StringComparison.Ordinal)
                        ? json.Replace("\"outputFile\":\"page.md\"", replacement, StringComparison.Ordinal)
                        : json.Replace(
                            "\"sourceUri\":\"https://example.com/docs/page\"",
                            replacement,
                            StringComparison.Ordinal);
            File.WriteAllText(Path.Combine(root, ExtractionManifestValidator.ManifestFileName), json);

            InvalidDataException exception = Assert.Throws<InvalidDataException>(
                () => new DirectoryExtractionManifestStore().Load(root));

            Assert.Contains(root, exception.Message, StringComparison.Ordinal);
            Assert.Contains(expectedField, exception.Message, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void Load_DuplicateMappings_Throws()
    {
        string root = NewTemporaryDirectory();
        try
        {
            string json = """
                {"schemaVersion":1,"sourceBaseUri":"https://example.com/docs/","entries":[
                {"sourceUri":"https://example.com/docs/page","outputFile":"page.md"},
                {"sourceUri":"https://example.com/docs/page","outputFile":"other.md"}]}
                """;
            File.WriteAllText(Path.Combine(root, ExtractionManifestValidator.ManifestFileName), json);

            Assert.Throws<InvalidDataException>(() => new DirectoryExtractionManifestStore().Load(root));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void Load_MissingRequiredField_Throws()
    {
        string root = NewTemporaryDirectory();
        try
        {
            File.WriteAllText(
                Path.Combine(root, ExtractionManifestValidator.ManifestFileName),
                """{"schemaVersion":1,"entries":[]}""");

            InvalidDataException exception = Assert.Throws<InvalidDataException>(
                () => new DirectoryExtractionManifestStore().Load(root));

            Assert.Contains("sourceBaseUri", exception.Message, StringComparison.Ordinal);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Theory]
    [InlineData("/absolute.md")]
    [InlineData("C:/absolute.md")]
    [InlineData(".")]
    [InlineData("./page.md")]
    [InlineData("folder//page.md")]
    [InlineData("folder\\page.md")]
    public void Validate_InvalidOutputPath_Throws(string outputFile)
    {
        ExtractionManifest manifest = new(
            1,
            new Uri("https://example.com/docs/"),
            [new(new Uri("https://example.com/docs/page"), outputFile)]);

        InvalidDataException exception = Assert.Throws<InvalidDataException>(
            () => ExtractionManifestValidator.ValidateAndNormalize(manifest, "/output"));

        Assert.Contains("outputFile", exception.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void Validate_DuplicateOutputMapping_Throws()
    {
        ExtractionManifest manifest = new(
            1,
            new Uri("https://example.com/docs/"),
            [
                new(new Uri("https://example.com/docs/first"), "same.md"),
                new(new Uri("https://example.com/docs/second"), "same.md"),
            ]);

        Assert.Throws<InvalidDataException>(
            () => ExtractionManifestValidator.ValidateAndNormalize(manifest, "/output"));
    }

    [Fact]
    public void Validate_AllowsSourceUriOutsideSourceBase()
    {
        ExtractionManifest manifest = new(
            1,
            new Uri("https://example.com/docs/"),
            [new(
                new Uri("https://example.com/images/logo.png"),
                "images/logo.png")]);

        ExtractionManifest normalized =
            ExtractionManifestValidator.ValidateAndNormalize(
                manifest,
                "/output");

        Assert.Single(normalized.Entries);
    }

    [Theory]
    [InlineData("page.md?lang=en")]
    [InlineData("page.md#section")]
    public void GetOutputFile_RemovesQueryAndFragment(string translatedPath)
    {
        string result = ExtractionManifestValidator.GetRelativeOutputPath(
            "/output",
            new Uri(translatedPath, UriKind.Relative));

        Assert.Equal("page.md", result);
    }

    [Fact]
    public void GetRelativeOutputPath_DecodesPhysicalFileNameOnce()
    {
        string result = ExtractionManifestValidator.GetRelativeOutputPath(
            "/output",
            new Uri("report%20final.md", UriKind.Relative));

        Assert.Equal("report final.md", result);
    }

    [Fact]
    public void GetRelativeOutputPath_DoesNotDoubleDecodeReservedName()
    {
        string result = ExtractionManifestValidator.GetRelativeOutputPath(
            "/output",
            new Uri("%2565xtraction-manifest.json", UriKind.Relative));

        Assert.Equal("%65xtraction-manifest.json", result);
    }

    [Fact]
    public void Load_MalformedOrMissingJson_ThrowsRootSpecificError()
    {
        string root = NewTemporaryDirectory();
        try
        {
            DirectoryExtractionManifestStore store = new();
            Assert.Contains(root, Assert.Throws<InvalidDataException>(() => store.Load(root)).Message, StringComparison.Ordinal);

            File.WriteAllText(Path.Combine(root, ExtractionManifestValidator.ManifestFileName), "{not json");
            Assert.Throws<InvalidDataException>(() => store.Load(root));
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    private static string ValidJson()
        => """
            {"schemaVersion":1,"sourceBaseUri":"https://example.com/docs/","entries":[{"sourceUri":"https://example.com/docs/page","outputFile":"page.md"}]}
            """;

    private static string NewTemporaryDirectory()
    {
        string root = Path.Combine(Path.GetTempPath(), "DDBMarkdown-manifest-tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        return root;
    }
}
