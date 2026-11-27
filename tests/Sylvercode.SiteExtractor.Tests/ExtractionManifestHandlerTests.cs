using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.Manifest;
using Sylvercode.SiteExtractor.Observation;
using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Store;
using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Tests;

/// <summary>Tests for <see cref="ExtractionManifestHandler"/> manifest lifecycle handling.</summary>
public class ExtractionManifestHandlerTests_OnNext
{
    /// <summary>Verifies that extraction startup loads configured linked manifests into the catalog.</summary>
    [Fact]
    public void ExtractionStarted_LoadsLinkedCatalog()
    {
        MemoryExtractionManifestStore store = new();
        store.Write(
            "/linked",
            new ExtractionManifest(
                1,
                new Uri("https://other.example/docs/"),
                [new(new Uri("https://other.example/docs/page"), "page.md")]));
        ExtractionManifestCatalog catalog = new();
        ExtractionManifestHandler handler = CreateHandler(
            store,
            catalog,
            new LinkedExtractionOptions { OutputDirectory = "/linked" });

        handler.OnNext(
            new ExtractionStarted(new Uri("https://example.com/docs/start")));

        ExtractionManifest linked = Assert.Single(catalog.LinkedManifests).Value;
        Assert.Equal(
            "https://other.example/docs/",
            linked.SourceBaseUri.AbsoluteUri);
    }

    /// <summary>Verifies that successful completion writes only output-producing resources.</summary>
    [Fact]
    public void ExtractionCompleted_WritesCurrentManifest()
    {
        MemoryExtractionManifestStore store = new();
        ExtractionManifestHandler handler = CreateHandler(
            store,
            new ExtractionManifestCatalog());
        ResourceRepository resources = [];
        Resource output = resources.Add(
            new Uri("https://example.com/docs/page")).resource;
        output.UriTranslater = new FixedTranslater("page.md");
        output.MarkAsPulled(ResourceExtractionState.OutputProduced);
        resources.Add(new Uri("https://example.com/docs/no-output"));

        ExtractionCompleted completed = new(resources);
        handler.OnNext(completed);
        completed.Commit();

        ExtractionManifest manifest = Assert.Single(store.Manifests).Value;
        ExtractionManifestEntry entry = Assert.Single(manifest.Entries);
        Assert.Equal("page.md", entry.OutputFile);
    }

    [Fact]
    public void ExtractionCompleted_DefaultFilterRemovesQueryFromSourceIdentity()
    {
        MemoryExtractionManifestStore store = new();
        ExtractionManifestHandler handler = CreateHandler(
            store,
            new ExtractionManifestCatalog());
        ResourceRepository resources = [];
        Resource output = resources.Add(
            new Uri("https://example.com/docs/page?lang=en")).resource;
        output.UriTranslater = new FixedTranslater("page.md?lang=en");
        output.MarkAsPulled(
            ResourceExtractionState.OutputProduced);

        ExtractionCompleted completed = new(resources);
        handler.OnNext(completed);
        completed.Commit();

        ExtractionManifestEntry entry = Assert.Single(
            Assert.Single(store.Manifests).Value.Entries);
        Assert.Equal("https://example.com/docs/page", entry.SourceUri.AbsoluteUri);
        Assert.Equal("page.md", entry.OutputFile);
    }

    [Fact]
    public void ExtractionCompleted_PreserveAllFilterKeepsQueryInSourceIdentity()
    {
        MemoryExtractionManifestStore store = new();
        ExtractionManifestHandler handler = CreateHandler(
            store,
            new ExtractionManifestCatalog(),
            queryIdentityFilter: new PreserveAllUriQueryIdentityFilter());
        ResourceRepository resources = [];
        Resource output = resources.Add(
            new Uri("https://example.com/docs/page?lang=en")).resource;
        output.UriTranslater = new FixedTranslater("page.md?lang=en");
        output.MarkAsPulled(
            ResourceExtractionState.OutputProduced);

        ExtractionCompleted completed = new(resources);
        handler.OnNext(completed);
        completed.Commit();

        ExtractionManifestEntry entry = Assert.Single(
            Assert.Single(store.Manifests).Value.Entries);
        Assert.Equal("https://example.com/docs/page?lang=en", entry.SourceUri.AbsoluteUri);
    }

    /// <summary>Verifies that a failure event never writes a manifest.</summary>
    [Fact]
    public void ExtractionFailed_DoesNotWriteManifest()
    {
        MemoryExtractionManifestStore store = new();
        ExtractionManifestHandler handler = CreateHandler(
            store,
            new ExtractionManifestCatalog());

        handler.OnNext(
            new ExtractionFailed(
                new InvalidOperationException("Failed"),
                Resource: null,
                Operation: "Process"));

        Assert.Empty(store.Manifests);
    }

    private static ExtractionManifestHandler CreateHandler(
        MemoryExtractionManifestStore store,
        ExtractionManifestCatalog catalog,
        params LinkedExtractionOptions[] linkedExtractions)
        => CreateHandler(
            store,
            catalog,
            new RemoveAllUriQueryIdentityFilter(),
            linkedExtractions);

    private static ExtractionManifestHandler CreateHandler(
        MemoryExtractionManifestStore store,
        ExtractionManifestCatalog catalog,
        IUriQueryIdentityFilter queryIdentityFilter,
        params LinkedExtractionOptions[] linkedExtractions)
    {
        SiteExtractorOptions options = new()
        {
            SourceAuthority = "https://example.com",
            SourceBasePath = "/docs",
            OutputDirectory = "/output",
        };
        foreach (LinkedExtractionOptions linkedExtraction in linkedExtractions)
            options.LinkedExtractions.Add(linkedExtraction);

        return new ExtractionManifestHandler(
            Options.Create(options),
            store,
            catalog,
            queryIdentityFilter);
    }

    private sealed class FixedTranslater(string outputFile) : IResourceUriTranslater
    {
        public Uri Translate(Resource resource, Uri uri)
            => new(outputFile, UriKind.Relative);
    }
}
