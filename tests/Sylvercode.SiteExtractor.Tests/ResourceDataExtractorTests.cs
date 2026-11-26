using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Resources.Processors;
using Sylvercode.SiteExtractor.Sources;
using Sylvercode.SiteExtractor.Store;
using Sylvercode.SiteExtractor.UriUtils;
using Sylvercode.SiteExtractor.Tests.Mocks;
using Sylvercode.SiteExtractor.Tests.Stubs;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.SiteExtractor.Tests;

/// <summary>Tests for <see cref="ResourceDataExtractor{TData}.Extract"/> producing structured nodes from documents.</summary>
public class ResourceDataExtractorTests_Extract
{
    /// <summary>Creates a default DI host with mock source, store, extractor, and serializer.</summary>
    /// <param name="calTracker">The tracker used to capture mock call order.</param>
    /// <param name="dataReturnsNull">When <see langword="true"/>, the mock source returns <see langword="null"/> for all URIs.</param>
    /// <param name="extractNothing">When <see langword="true"/>, the mock extractor produces no nodes.</param>
    /// <returns>A built <see cref="IHost"/> ready for test use.</returns>
    public static IHost GetDefaultHost(
        MockCallTracker calTracker,
        bool dataReturnsNull = false,
        bool extractNothing = false,
        IUriRedirector? uriRedirector = null)
    {
        IHostBuilder builder = Host.CreateDefaultBuilder();

        builder.ConfigureServices(services =>
        {
            services.AddSingleton(calTracker);

            services.AddSingleton<ISiteSource<Uri?>>(provider =>
            {
                var tracker = provider.GetRequiredService<MockCallTracker>();
                return new SiteSourceMock(tracker, dataReturnsNull);
            });
            services.AddSingleton<IDataStore, DataStoreMock>();
            services.AddSingleton<IExtractor<Uri>>(provider =>
            {
                var tracker = provider.GetRequiredService<MockCallTracker>();
                return new ExtractorMock(tracker, extractNothing);
            });
            services.AddSingleton<IStructDocSerializer, StructDocSerializerMock>();

            if (uriRedirector is not null)
                services.AddSingleton(uriRedirector);

            services.AddSingleton<IResourceDataExtractor<Uri>, ResourceDataExtractor<Uri>>();
        });

        return builder.Build();
    }

    /// <summary>Verifies that extracting an existing resource with no referenced URIs returns an unfinished result with a single node.</summary>
    [Fact]
    public void ExtractExistingWithNoReference_Unfinish()
    {
        // Given
        MockCallTracker callTracker = new();
        IHost host = GetDefaultHost(callTracker);
        var extractor = host.Services.GetRequiredService<IResourceDataExtractor<Uri>>();
        Uri input = new("test://mock-input");

        // When
        IResourceProcessorResult result = extractor.Extract(new Resource(input, isPullable: true), new ResourceRepository());

        // Then
        var dataResult = Assert.IsType<DataExtractedProcessorResult<Uri>>(result);
        var resultNode = Assert.IsType<UriNode>(Assert.Single(dataResult.Result.StructDocNodes));
        Assert.Equal(input.ToString(), resultNode.Uri.ToString());
        Assert.NotNull(dataResult.ResourceUriTranslaterToSet);
        Assert.Empty(dataResult.Referencers);
        Assert.True(dataResult.IsUnfinished);

        Assert.Collection(callTracker.Calls,
            call =>
            {
                Assert.Equal(nameof(ISiteSource.GetData), call.MethodName);
                Assert.Equal(input, call.Arguments[0]);
            },
            call =>
            {
                Assert.Equal(nameof(IExtractor<Uri>.Extract), call.MethodName);
                Assert.Equal(input, call.Arguments[0]);
            });
    }

    /// <summary>Verifies that when the source returns no data the result is immediately finished with no URI translater.</summary>
    [Fact]
    public void ExtractNotFound_Finished()
    {
        // Given
        MockCallTracker callTracker = new();
        IHost host = GetDefaultHost(callTracker, dataReturnsNull: true);
        var extractor = host.Services.GetRequiredService<IResourceDataExtractor<Uri>>();
        Uri input = new("test://mock-input");

        // When
        IResourceProcessorResult result = extractor.Extract(new Resource(input, isPullable: true), new ResourceRepository());

        // Then
        Assert.False(result.IsUnfinished);
        Assert.Null(result.ResourceUriTranslaterToSet);
        Assert.Equal(ResourceExtractionState.NoSourceData, result.ExtractionState);

        Assert.Collection(callTracker.Calls,
            call =>
            {
                Assert.Equal(nameof(ISiteSource.GetData), call.MethodName);
                Assert.Equal(input, call.Arguments[0]);
            });
    }

    /// <summary>Verifies that when the extractor produces no nodes the result is immediately finished with no URI translater.</summary>
    [Fact]
    public void ExtractNothing_Finished()
    {
        // Given
        MockCallTracker callTracker = new();
        IHost host = GetDefaultHost(callTracker, extractNothing: true);
        var extractor = host.Services.GetRequiredService<IResourceDataExtractor<Uri>>();
        Uri input = new("test://mock-input");

        // When
        IResourceProcessorResult result = extractor.Extract(new Resource(input, isPullable: true), new ResourceRepository());

        // Then
        Assert.False(result.IsUnfinished);
        Assert.Null(result.ResourceUriTranslaterToSet);
        Assert.Equal(ResourceExtractionState.NoOutput, result.ExtractionState);

        Assert.Collection(callTracker.Calls,
            call =>
            {
                Assert.Equal(nameof(ISiteSource.GetData), call.MethodName);
                Assert.Equal(input, call.Arguments[0]);
            },
            call =>
            {
                Assert.Equal(nameof(IExtractor<Uri>.Extract), call.MethodName);
                Assert.Equal(input, call.Arguments[0]);
            });
    }

    /// <summary>Verifies that extracted reference URIs from query parameters appear as referencers in the unfinished result.</summary>
    [Fact]
    public void ExtractExistingWithReferences_Unfinish()
    {
        // Given
        MockCallTracker callTracker = new();
        IHost host = GetDefaultHost(callTracker);
        var extractor = host.Services.GetRequiredService<IResourceDataExtractor<Uri>>();
        Uri input = new("test://mock-input?referencer1=foo&referencer2=bar");

        // When
        IResourceProcessorResult result = extractor.Extract(new Resource(input, isPullable: true), new ResourceRepository());

        // Then
        var dataResult = Assert.IsType<DataExtractedProcessorResult<Uri>>(result);
        var resultNode = Assert.IsType<UriNode>(Assert.Single(dataResult.Result.StructDocNodes));
        Assert.Equal(input.ToString(), resultNode.Uri.ToString());
        Assert.NotNull(dataResult.ResourceUriTranslaterToSet);
        Assert.Equal(ResourceExtractionState.NotProcessed, dataResult.ExtractionState);
        Assert.Collection(dataResult.Referencers,
            referencer => Assert.Equal("test://mock-input/foo", referencer.GetReference()),
            referencer => Assert.Equal("test://mock-input/bar", referencer.GetReference()));
        Assert.True(dataResult.IsUnfinished);

        Assert.Collection(callTracker.Calls,
            call =>
            {
                Assert.Equal(nameof(ISiteSource.GetData), call.MethodName);
                Assert.Equal(input, call.Arguments[0]);
            },
            call =>
            {
                Assert.Equal(nameof(IExtractor<Uri>.Extract), call.MethodName);
                Assert.Equal(input, call.Arguments[0]);
            });
    }

    /// <summary>Verifies that each extracted reference is checked and server redirects are applied before dependency tracking.</summary>
    [Fact]
    public void ExtractExistingWithReferences_UsesServerRedirects()
    {
        MockCallTracker callTracker = new();
        MappedUriRedirector uriRedirector = new(new Dictionary<string, string>
        {
            ["https://example.test/old"] = "https://cdn.example.test/new"
        });
        using IHost host = GetDefaultHost(callTracker, uriRedirector: uriRedirector);
        var extractor = host.Services.GetRequiredService<IResourceDataExtractor<Uri>>();
        Uri input = new("https://example.test/page?referencer1=old&referencer2=unchanged");

        IResourceProcessorResult result = extractor.Extract(new Resource(input, isPullable: true), new ResourceRepository());

        var dataResult = Assert.IsType<DataExtractedProcessorResult<Uri>>(result);
        Assert.Collection(dataResult.Referencers,
            referencer => Assert.Equal("https://cdn.example.test/new", referencer.GetReference()),
            referencer => Assert.Equal("https://example.test/unchanged", referencer.GetReference()));
        Assert.Equal(
            ["https://example.test/old", "https://example.test/unchanged"],
            uriRedirector.RequestedUris);
    }

    private sealed class MappedUriRedirector(Dictionary<string, string> redirects) : IUriRedirector
    {
        public List<string> RequestedUris { get; } = [];

        public bool RedirectUri(string uri, [NotNullWhen(true)] out string? redirectedUri)
        {
            RequestedUris.Add(uri);
            return redirects.TryGetValue(uri, out redirectedUri);
        }
    }

    /// <summary>Verifies that metadata encoded in the source URI query string is propagated to the result.</summary>
    [Fact]
    public void ExtractWithMetadata_MetatdaFilled()
    {
        MockCallTracker callTracker = new();
        IHost host = GetDefaultHost(callTracker);
        var extractor = host.Services.GetRequiredService<IResourceDataExtractor<Uri>>();
        Uri input = new($"test://mock-input?{ExtractorMock.MetaKey}1=data1&{ExtractorMock.MetaKey}2=data2");

        // When
        IResourceProcessorResult result = extractor.Extract(new Resource(input, isPullable: true), new ResourceRepository());

        // Then
        Assert.Collection(result.NewMetadata,
            md =>
            {
                Assert.Equal(ExtractorMock.MetaKey + "1", md.Key);
                Assert.Equal(md.Key, md.Value.Name);
                Assert.Equal("data1", md.Value.GetStrValue());
            },
            md =>
            {
                Assert.Equal(ExtractorMock.MetaKey + "2", md.Key);
                Assert.Equal(md.Key, md.Value.Name);
                Assert.Equal("data2", md.Value.GetStrValue());
            });

    }
}

/// <summary>Tests for <see cref="ResourceDataExtractor{TData}.ContinueProcess"/> serialising extracted nodes to the data store.</summary>
public class ResourceDataExtractorTests_ContinueProcess
{
    /// <summary>Verifies that continuing after a standard extraction writes the root node to the store and returns a finished result.</summary>
    [Fact]
    public void StadardNode_Finished()
    {
        // Given
        MockCallTracker calTracker = new();
        IHost host = ResourceDataExtractorTests_Extract.GetDefaultHost(calTracker);
        var extractor = host.Services.GetRequiredService<IResourceDataExtractor<Uri>>();
        Uri input = new("test://mock-input");
        Resource resource = new(input, isPullable: true);
        var result = (DataExtractedProcessorResult<Uri>)extractor.Extract(resource, new ResourceRepository());
        calTracker.Calls.Clear();

        // When
        IResourceProcessorResult subResult = result.ContinueProcess(resource);

        // Then
        Assert.False(subResult.IsUnfinished);

        Assert.Collection(calTracker.Calls,
            call => Assert.Equal(nameof(IDataStore.GetStreamWriter), call.MethodName),
            call =>
            {
                Assert.Equal(nameof(IStructDocSerializer.Serialize), call.MethodName);
                Assert.Equal(result.Result.StructDocNodes[0], call.Arguments[1]);
            });
    }
}
