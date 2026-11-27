using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sylvercode.SiteExtractor.Manifest;
using Sylvercode.SiteExtractor.Observation;
using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Store;
using Sylvercode.SiteExtractor.Tests.Mocks;
using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Tests;

public class SiteExtractorManifestTests
{
    [Fact]
    public void Extract_OnlyWritesAfterContinuation_AndUsesFinalOutputName()
    {
        ResourceProcessorMock processor = new(returnsFinishedResultByDefault: false);
        IHost host = CreateHost(processor);
        MemoryExtractionManifestStore store =
            host.Services.GetRequiredService<MemoryExtractionManifestStore>();
        processor.AddResult((processor, resource) =>
        {
            ResourceProcessorResultMock continuation = new(processor, resource)
            {
                ExtractionState = ResourceExtractionState.OutputProduced,
            };
            ResourceProcessorResultMock initial = new(
                processor,
                resource,
                continuProcessResult: continuation,
                beforeContinue: () =>
                    Assert.Empty(store.Manifests))
            {
                ResourceUriTranslaterToSet = new HeadingTranslater(),
                ExtractionState = ResourceExtractionState.ExtractionPrepared,
            };
            initial.NewMetadata.AddMetadata("title", "Final Heading");
            return initial;
        });

        host.Services.GetRequiredService<SiteExtractor>().Extract(new Uri("https://example.com/docs/page"));

        ExtractionManifest manifest = Assert.Single(store.Manifests).Value;
        Assert.Equal("Final Heading.md", Assert.Single(manifest.Entries).OutputFile);
    }

    [Fact]
    public void Extract_IncludesProducedPagesAndAssetsButExcludesNoOutputResources()
    {
        ResourceProcessorMock processor = new(returnsFinishedResultByDefault: false);
        IHost host = CreateHost(processor);
        processor.AddResult((processor, resource) =>
        {
            return new ResourceProcessorResultMock(
                processor,
                resource,
                [
                    new Uri("https://example.com/docs/no-output"),
                    new Uri("https://example.com/docs/image.png"),
                    new Uri("https://external.example/image.png"),
                ])
            {
                ResourceUriTranslaterToSet = new FixedTranslater("page.md"),
                ExtractionState = ResourceExtractionState.OutputProduced,
                OutputKind = ResourceOutputKind.Document,
            };
        });
        processor.AddResult((processor, resource) => new ResourceProcessorResultMock(processor, resource));
        processor.AddResult((processor, resource) =>
        {
            return new ResourceProcessorResultMock(processor, resource)
            {
                ResourceUriTranslaterToSet = new FixedTranslater("assets/image.png"),
                ExtractionState = ResourceExtractionState.OutputProduced,
                OutputKind = ResourceOutputKind.Asset,
            };
        });

        host.Services.GetRequiredService<SiteExtractor>().Extract(new Uri("https://example.com/docs/page"));

        ExtractionManifest manifest = Assert.Single(
            host.Services.GetRequiredService<MemoryExtractionManifestStore>().Manifests).Value;
        Assert.Equal(
            ["assets/image.png", "page.md"],
            manifest.Entries.Select(entry => entry.OutputFile).ToArray());
    }

    [Fact]
    public void Extract_FailureLeavesExistingManifestUntouched()
    {
        ResourceProcessorMock processor = new(returnsFinishedResultByDefault: false);
        IHost host = CreateHost(processor);
        MemoryExtractionManifestStore store =
            host.Services.GetRequiredService<MemoryExtractionManifestStore>();
        store.Write(
            Root,
            new ExtractionManifest(
                1,
                new Uri("https://example.com/docs/"),
                [new(new Uri("https://example.com/docs/legacy"), "legacy.md")]));
        processor.AddResult((processor, resource) =>
            new ResourceProcessorResultMock(
                processor,
                resource,
                continuProcessResult: new ResourceProcessorResultMock(processor, resource),
                beforeContinue: () => throw new InvalidOperationException("Continuation failed.")));

        Assert.Throws<InvalidOperationException>(
            () => host.Services.GetRequiredService<SiteExtractor>().Extract(
                new Uri("https://example.com/docs/page")));

        Assert.Equal("legacy.md", Assert.Single(store.Load(Root).Entries).OutputFile);
    }

    [Fact]
    public void Extract_CompletionObserverFailureLeavesExistingManifestUntouched()
    {
        ResourceProcessorMock processor = new();
        processor.AddResult((processor, resource) =>
            new ResourceProcessorResultMock(processor, resource)
            {
                ResourceUriTranslaterToSet = new FixedTranslater("new.md"),
                ExtractionState = ResourceExtractionState.OutputProduced,
            });
        IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.Configure<SiteExtractorOptions>(options =>
                {
                    options.SourceAuthority = "https://example.com";
                    options.SourceBasePath = "/docs";
                    options.OutputDirectory = Root;
                });
                services.AddResourceProcessor(
                    processor,
                    new UriMatcherByBase(new Uri("https://example.com/docs/")));
                services.AddResourceProcessorProvider();
                services.AddMemoryDataStore();
                services.AddMemoryExtractionManifestStore();
                services.AddSingleton<IExtractionObserver>(
                    new FailingCompletionObserver());
                services.AddSiteExtractor();
            })
            .Build();
        MemoryExtractionManifestStore store =
            host.Services.GetRequiredService<MemoryExtractionManifestStore>();
        store.Write(
            Root,
            new ExtractionManifest(
                1,
                new Uri("https://example.com/docs/"),
                [new(new Uri("https://example.com/docs/legacy"), "legacy.md")]));

        Assert.Throws<InvalidOperationException>(
            () => host.Services.GetRequiredService<SiteExtractor>().Extract(
                new Uri("https://example.com/docs/new")));

        Assert.Equal("legacy.md", Assert.Single(store.Load(Root).Entries).OutputFile);
    }

    [Fact]
    public void Extract_ProcessingFailureMarksResourceFailed()
    {
        ResourceProcessorMock processor = new();
        Resource? failedResource = null;
        processor.AddResult((processor, resource) =>
        {
            failedResource = resource;
            throw new InvalidOperationException("Processing failed.");
        });
        IHost host = CreateHost(processor);

        Assert.Throws<InvalidOperationException>(
            () => host.Services.GetRequiredService<SiteExtractor>().Extract(
                new Uri("https://example.com/docs/page")));

        Assert.Equal(ResourceExtractionState.Failed, failedResource!.State.ExtractionState);
    }

    [Fact]
    public void Extract_CopiedAssetOutsideSourceBase_IsIncluded()
    {
        ResourceProcessorMock processor = new(returnsFinishedResultByDefault: false);
        IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.Configure<SiteExtractorOptions>(options =>
                {
                    options.SourceAuthority = "https://example.com";
                    options.SourceBasePath = "/docs";
                    options.OutputDirectory = Root;
                });
                services.AddResourceProcessor(
                    processor,
                    new UriMatcherByBase(new Uri("https://example.com/docs/")));
                services.AddResourceProcessor(processor, ImageUriMatcher.Default);
                services.AddResourceProcessorProvider();
                services.AddMemoryDataStore();
                services.AddMemoryExtractionManifestStore();
                services.AddSiteExtractor();
            })
            .Build();
        processor.AddResult((processor, resource) =>
            new ResourceProcessorResultMock(
                processor,
                resource,
                [new Uri("https://example.com/images/logo.png")])
            {
                ResourceUriTranslaterToSet = new FixedTranslater("page.md"),
                ExtractionState = ResourceExtractionState.OutputProduced,
                OutputKind = ResourceOutputKind.Document,
            });
        processor.AddResult((processor, resource) =>
            new ResourceProcessorResultMock(processor, resource)
            {
                ResourceUriTranslaterToSet = new FixedTranslater("images/logo.png"),
                ExtractionState = ResourceExtractionState.OutputProduced,
                OutputKind = ResourceOutputKind.Asset,
            });

        host.Services.GetRequiredService<SiteExtractor>().Extract(
            new Uri("https://example.com/docs/page"));

        ExtractionManifest manifest = Assert.Single(
            host.Services.GetRequiredService<MemoryExtractionManifestStore>().Manifests).Value;
        Assert.Collection(
            manifest.Entries,
            entry => Assert.Equal("page.md", entry.OutputFile),
            entry => Assert.Equal("images/logo.png", entry.OutputFile));
    }

    [Fact]
    public void Extract_MissingLinkedManifestFailsBeforeProcessorRuns()
    {
        ResourceProcessorMock processor = new();
        IHost host = CreateHost(
            processor,
            new LinkedExtractionOptions { OutputDirectory = "/linked-output" });

        InvalidDataException exception = Assert.Throws<InvalidDataException>(
            () => host.Services.GetRequiredService<SiteExtractor>().Extract(
                new Uri("https://example.com/docs/page")));

        Assert.Contains("/linked-output", exception.Message, StringComparison.Ordinal);
        Assert.Equal(0, processor.ProcessCount);
    }

    [Fact]
    public void Extract_NestedLinkedOutputRootFailsBeforeProcessorRuns()
    {
        ResourceProcessorMock processor = new();
        IHost host = CreateHost(
            processor,
            new LinkedExtractionOptions { OutputDirectory = $"{Root}/linked" });

        InvalidOperationException exception =
            Assert.Throws<InvalidOperationException>(
                () => host.Services.GetRequiredService<SiteExtractor>().Extract(
                    new Uri("https://example.com/docs/page")));

        Assert.Contains("linked", exception.Message, StringComparison.Ordinal);
        Assert.Equal(0, processor.ProcessCount);
    }

    [Fact]
    public void Extract_DuplicateLinkedRootsFailBeforeProcessorRuns()
    {
        ResourceProcessorMock processor = new();
        IHost host = CreateHost(
            processor,
            new LinkedExtractionOptions { OutputDirectory = "/linked" },
            new LinkedExtractionOptions { OutputDirectory = "/linked/" });

        Assert.Throws<InvalidOperationException>(
            () => host.Services.GetRequiredService<SiteExtractor>().Extract(
                new Uri("https://example.com/docs/page")));
        Assert.Equal(0, processor.ProcessCount);
    }

    [Fact]
    public void Extract_OverlappingLinkedSourceBaseFailsBeforeProcessorRuns()
    {
        ResourceProcessorMock processor = new();
        IHost host = CreateHost(
            processor,
            new LinkedExtractionOptions { OutputDirectory = "/linked" });
        host.Services.GetRequiredService<MemoryExtractionManifestStore>().Write(
            "/linked",
            new ExtractionManifest(
                1,
                new Uri("https://example.com/docs/other/"),
                []));

        Assert.Throws<InvalidOperationException>(
            () => host.Services.GetRequiredService<SiteExtractor>().Extract(
                new Uri("https://example.com/docs/page")));
        Assert.Equal(0, processor.ProcessCount);
    }

    [Fact]
    public void Extract_RetainsLoadedLinkedManifestsWithoutSeedingCrawl()
    {
        ResourceProcessorMock processor = new();
        IHost host = CreateHost(
            processor,
            new LinkedExtractionOptions { OutputDirectory = "/linked" });
        host.Services.GetRequiredService<MemoryExtractionManifestStore>().Write(
            "/linked",
            new ExtractionManifest(
                1,
                new Uri("https://other.example/docs/"),
                [new(new Uri("https://other.example/docs/linked"), "linked.md")]));

        host.Services.GetRequiredService<SiteExtractor>().Extract(
            new Uri("https://example.com/docs/page"));

        ExtractionManifest linked = Assert.Single(
            host.Services.GetRequiredService<IExtractionManifestCatalog>().LinkedManifests).Value;
        Assert.Equal("linked.md", Assert.Single(linked.Entries).OutputFile);
        Assert.Equal(1, processor.ProcessCount);
        Assert.Empty(Assert.Single(
            host.Services.GetRequiredService<MemoryExtractionManifestStore>().Manifests,
            pair => pair.Key == Root).Value.Entries);
    }

    /// <summary>Verifies that consecutive extractions do not retain a previous run context.</summary>
    [Fact]
    public void Extract_RepeatedRunsBuildIndependentManifests()
    {
        ResourceProcessorMock processor = new(returnsFinishedResultByDefault: false);
        processor.AddResult((processor, resource) =>
            new ResourceProcessorResultMock(processor, resource)
            {
                ResourceUriTranslaterToSet = new FixedTranslater("first.md"),
                ExtractionState = ResourceExtractionState.OutputProduced,
            });
        processor.AddResult((processor, resource) =>
            new ResourceProcessorResultMock(processor, resource)
            {
                ResourceUriTranslaterToSet = new FixedTranslater("second.md"),
                ExtractionState = ResourceExtractionState.OutputProduced,
            });
        IHost host = CreateHost(processor);
        SiteExtractor extractor = host.Services.GetRequiredService<SiteExtractor>();

        extractor.Extract(new Uri("https://example.com/docs/first"));
        extractor.Extract(new Uri("https://example.com/docs/second"));

        ExtractionManifest manifest = Assert.Single(
            host.Services.GetRequiredService<MemoryExtractionManifestStore>().Manifests).Value;
        ExtractionManifestEntry entry = Assert.Single(manifest.Entries);
        Assert.Equal("https://example.com/docs/second", entry.SourceUri.AbsoluteUri);
        Assert.Equal("second.md", entry.OutputFile);
    }

    private const string Root = "/output";

    private static IHost CreateHost(
        ResourceProcessorMock processor,
        params LinkedExtractionOptions[] linkedExtractions)
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                services.Configure<SiteExtractorOptions>(options =>
                {
                    options.SourceAuthority = "https://example.com";
                    options.SourceBasePath = "/docs";
                    options.OutputDirectory = Root;
                    foreach (LinkedExtractionOptions linkedExtraction in linkedExtractions)
                        options.LinkedExtractions.Add(linkedExtraction);
                });
                services.AddResourceProcessor(
                    processor,
                    new UriMatcherByBase(new Uri("https://example.com/docs/")));
                services.AddResourceProcessorProvider();
                services.AddMemoryDataStore();
                services.AddMemoryExtractionManifestStore();
                services.AddSiteExtractor();
            })
            .Build();
    }

    private sealed class FixedTranslater(string outputFile) : IResourceUriTranslater
    {
        public Uri Translate(Resource resource, Uri uri) => new(outputFile, UriKind.Relative);
    }

    private sealed class HeadingTranslater : IResourceUriTranslater
    {
        public Uri Translate(Resource resource, Uri uri)
            => new(resource.Metadata.GetStrValue("title") + ".md", UriKind.Relative);
    }

    private sealed class FailingCompletionObserver : IExtractionObserver
    {
        public void OnNext(ExtractionEvent value)
        {
            if (value is ExtractionCompleted)
                throw new InvalidOperationException("Completion observer failed.");
        }
    }

}
