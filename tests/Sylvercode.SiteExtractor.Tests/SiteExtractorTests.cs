using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sylvercode.SiteExtractor.Resources.Processors;
using Sylvercode.SiteExtractor.Tests.Mocks;
using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Tests;

public class SiteExtractorTests_Extract
{
    public static IHost GetDefaultHost(IResourceProcessor resourceProcessor)
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddResourceProcessor(
                    resourceProcessor,
                    new UriMatcherByBase(new Uri("https://example.com")));
                services.AddResourceProcessorProvider();
                services.AddSingleton<SiteExtractor>();
            })
            .Build();
    }

    [Fact]
    public void SingleStepNoDependancy_ReturnFinish()
    {
        // Given
        ResourceProcessorMock resourceProcessor = new(returnsFinishedResultByDefault: false);
        IHost host = GetDefaultHost(resourceProcessor);
        resourceProcessor.AddResult((p, r) => new ResourceProcessorResultMock(p, r));

        var extractor = host.Services.GetRequiredService<SiteExtractor>();

        // When
        extractor.Extract(new Uri("https://example.com/test"));

        // Then
        Assert.False(resourceProcessor.HasResults);
    }

    [Fact]
    public void SingleStepWithDependancy_ReturnFinish()
    {
        // Given
        ResourceProcessorMock resourceProcessor = new(returnsFinishedResultByDefault: false);
        IHost host = GetDefaultHost(resourceProcessor);
        resourceProcessor.AddResult((p, r) =>
            new ResourceProcessorResultMock(
                p, r,
                [new Uri("https://example.com/dep1"), new Uri("https://example.com/dep2")]));
        resourceProcessor.AddResult((p, r) => new ResourceProcessorResultMock(p, r));
        resourceProcessor.AddResult((p, r) => new ResourceProcessorResultMock(p, r));

        var extractor = host.Services.GetRequiredService<SiteExtractor>();

        // When
        extractor.Extract(new Uri("https://example.com/test1"));

        // Then
        Assert.False(resourceProcessor.HasResults);
    }

    [Fact]
    public void MultiStepWithNoDependancy_CallContinue()
    {
        // Given
        MockCallTracker callTracker = new();
        ResourceProcessorMock resourceProcessor = new(returnsFinishedResultByDefault: false);
        IHost host = GetDefaultHost(resourceProcessor);
        resourceProcessor.AddResult((p, r) =>
            new ResourceProcessorResultMock(
                p, r, continuProcessResult: new ResourceProcessorResultMock(p, r), callTracker: callTracker));

        var extractor = host.Services.GetRequiredService<SiteExtractor>();

        // When
        extractor.Extract(new Uri("https://example.com/test1"));

        // Then
        Assert.False(resourceProcessor.HasResults);
        Assert.Collection(callTracker.Calls,
            call => Assert.Equal(nameof(IResourceProcessorResult.ContinueProcess), call.MethodName));
    }

    [Fact]
    public void MultiStepWithDependancy_CallContinue()
    {
        // Given
        MockCallTracker callTracker = new();
        ResourceProcessorMock resourceProcessor = new(returnsFinishedResultByDefault: false);
        IHost host = GetDefaultHost(resourceProcessor);
        resourceProcessor.AddResult((p, r) =>
            new ResourceProcessorResultMock(
                p, r,
                [new Uri("https://example.com/dep1"), new Uri("https://example.com/dep2")],
                new ResourceProcessorResultMock(p, r), callTracker));
        resourceProcessor.AddResult((p, r) => new ResourceProcessorResultMock(
            p,
            r,
            continuProcessResult: new ResourceProcessorResultMock(p, r),
            callTracker: callTracker));
        resourceProcessor.AddResult((p, r) => new ResourceProcessorResultMock(
            p,
            r,
            continuProcessResult: new ResourceProcessorResultMock(p, r),
            callTracker: callTracker));

        var extractor = host.Services.GetRequiredService<SiteExtractor>();

        // When
        extractor.Extract(new Uri("https://example.com/test1"));

        // Then
        Assert.False(resourceProcessor.HasResults);
        Assert.Collection(callTracker.Calls,
            call => Assert.Equal(nameof(IResourceProcessorResult.ContinueProcess), call.MethodName),
            call => Assert.Equal(nameof(IResourceProcessorResult.ContinueProcess), call.MethodName),
            call => Assert.Equal(nameof(IResourceProcessorResult.ContinueProcess), call.MethodName));
    }
}
