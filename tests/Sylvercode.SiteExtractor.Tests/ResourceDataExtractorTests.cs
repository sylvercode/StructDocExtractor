using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.Resources.Processors;
using Sylvercode.SiteExtractor.Sources;
using Sylvercode.SiteExtractor.Store;
using Sylvercode.SiteExtractor.Tests.Mocks;
using Sylvercode.SiteExtractor.Tests.Stubs;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Serialization;

namespace Sylvercode.SiteExtractor.Tests;

public class ResourceDataExtractorTests_Extract
{
    public static IHost GetDefaultHost(MockCallTracker calTracker, bool dataReturnsNull = false, bool extractNothing = false)
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

            services.AddSingleton<IResourceDataExtractor<Uri>, ResourceDataExtractor<Uri>>();
        });

        return builder.Build();
    }

    [Fact]
    public void ExtractExistingWithNoReference_Unfinish()
    {
        // Given
        MockCallTracker callTracker = new();
        IHost host = GetDefaultHost(callTracker);
        var extractor = host.Services.GetRequiredService<IResourceDataExtractor<Uri>>();
        Uri input = new("test://mock-input");

        // When
        IResourceProcessorResult result = extractor.Extract(new Resource(input), new Dictionary<Uri, Resource>());

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

    [Fact]
    public void ExtractNotFound_Finished()
    {
        // Given
        MockCallTracker callTracker = new();
        IHost host = GetDefaultHost(callTracker, dataReturnsNull: true);
        var extractor = host.Services.GetRequiredService<IResourceDataExtractor<Uri>>();
        Uri input = new("test://mock-input");

        // When
        IResourceProcessorResult result = extractor.Extract(new Resource(input), new Dictionary<Uri, Resource>());

        // Then
        Assert.False(result.IsUnfinished);
        Assert.Null(result.ResourceUriTranslaterToSet);

        Assert.Collection(callTracker.Calls,
            call =>
            {
                Assert.Equal(nameof(ISiteSource.GetData), call.MethodName);
                Assert.Equal(input, call.Arguments[0]);
            });
    }

    [Fact]
    public void ExtractNothing_Finished()
    {
        // Given
        MockCallTracker callTracker = new();
        IHost host = GetDefaultHost(callTracker, extractNothing: true);
        var extractor = host.Services.GetRequiredService<IResourceDataExtractor<Uri>>();
        Uri input = new("test://mock-input");

        // When
        IResourceProcessorResult result = extractor.Extract(new Resource(input), new Dictionary<Uri, Resource>());

        // Then
        Assert.False(result.IsUnfinished);
        Assert.Null(result.ResourceUriTranslaterToSet);

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
    [Fact]
    public void ExtractExistingWithReferences_Unfinish()
    {
        // Given
        MockCallTracker callTracker = new();
        IHost host = GetDefaultHost(callTracker);
        var extractor = host.Services.GetRequiredService<IResourceDataExtractor<Uri>>();
        Uri input = new("test://mock-input?referencer1=foo&referencer2=bar");

        // When
        IResourceProcessorResult result = extractor.Extract(new Resource(input), new Dictionary<Uri, Resource>());

        // Then
        var dataResult = Assert.IsType<DataExtractedProcessorResult<Uri>>(result);
        var resultNode = Assert.IsType<UriNode>(Assert.Single(dataResult.Result.StructDocNodes));
        Assert.Equal(input.ToString(), resultNode.Uri.ToString());
        Assert.NotNull(dataResult.ResourceUriTranslaterToSet);
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
}

public class ResourceDataExtractorContinueExtraction_Extract
{
    [Fact]
    public void StadardNode_Finished()
    {
        // Given
        MockCallTracker calTracker = new();
        IHost host = ResourceDataExtractorTests_Extract.GetDefaultHost(calTracker);
        var extractor = host.Services.GetRequiredService<IResourceDataExtractor<Uri>>();
        Uri input = new("test://mock-input");
        var result = (DataExtractedProcessorResult<Uri>)extractor.Extract(new Resource(input), new Dictionary<Uri, Resource>());
        calTracker.Calls.Clear();

        // When
        IResourceProcessorResult subResult = result.ContinueProcess();

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
