using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sylvercode.SiteExtractor.Observation;
using Sylvercode.SiteExtractor.Resources.Processors;
using Sylvercode.SiteExtractor.Tests.Mocks;
using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Tests;

/// <summary>End-to-end tests for <see cref="SiteExtractor.Extract"/> orchestrating the full pipeline.</summary>
public class SiteExtractorTests_Extract
{
    /// <summary>Creates a default DI host with the given resource processor bound to the example.com base URI.</summary>
    /// <param name="resourceProcessor">The processor to register for example.com resources.</param>
    /// <returns>A built <see cref="IHost"/> ready for test use.</returns>
    public static IHost GetDefaultHost(IResourceProcessor resourceProcessor)
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddResourceProcessor(
                    resourceProcessor,
                    new UriMatcherByBase(new Uri("https://example.com")));
                services.AddResourceProcessorProvider();
                services.AddSiteExtractor();
            })
            .Build();
    }

    /// <summary>Verifies that a single resource with no dependencies is processed and all queued results are consumed.</summary>
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

    /// <summary>Verifies that discovered dependencies are also processed until no queued results remain.</summary>
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

    /// <summary>Verifies that a multi-step result triggers <see cref="IResourceProcessorResult.ContinueProcess"/> exactly once.</summary>
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

    /// <summary>Verifies that multi-step results for both the root resource and its dependencies each trigger a continue call.</summary>
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

    /// <summary>Verifies that a custom observer receives the extraction lifecycle in order.</summary>
    [Fact]
    public void CustomObserver_ReceivesLifecycleEvents()
    {
        ResourceProcessorMock resourceProcessor = new();
        RecordingObserver observer = new();
        IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddResourceProcessor(
                    resourceProcessor,
                    new UriMatcherByBase(new Uri("https://example.com")));
                services.AddResourceProcessorProvider();
                services.AddSingleton<IExtractionObserver>(observer);
                services.AddSiteExtractor();
            })
            .Build();

        host.Services.GetRequiredService<SiteExtractor>().Extract(
            new Uri("https://example.com/test"));

        Assert.Collection(
            observer.Events,
            @event => Assert.IsType<ExtractionStarted>(@event),
            @event => Assert.IsType<ResourceProcessed>(@event),
            @event => Assert.IsType<ExtractionCompleted>(@event));
    }

    /// <summary>Verifies that observer error notifications do not replace the original processing exception.</summary>
    [Fact]
    public void CustomObserver_ReceivesFailureEvent()
    {
        ResourceProcessorMock resourceProcessor = new(returnsFinishedResultByDefault: false);
        resourceProcessor.AddResult(
            (_, _) => throw new InvalidOperationException("Processing failed."));
        RecordingObserver observer = new();
        IHost host = Host.CreateDefaultBuilder()
            .ConfigureServices((_, services) =>
            {
                services.AddResourceProcessor(
                    resourceProcessor,
                    new UriMatcherByBase(new Uri("https://example.com")));
                services.AddResourceProcessorProvider();
                services.AddSingleton<IExtractionObserver>(observer);
                services.AddSiteExtractor();
            })
            .Build();

        Assert.Throws<InvalidOperationException>(
            () => host.Services.GetRequiredService<SiteExtractor>().Extract(
                new Uri("https://example.com/test")));

        Assert.Collection(
            observer.Events,
            @event => Assert.IsType<ExtractionStarted>(@event),
            @event => Assert.IsType<ExtractionFailed>(@event));
        Assert.True(observer.Errored);
    }

    private sealed class RecordingObserver : IExtractionObserver
    {
        public List<ExtractionEvent> Events { get; } = [];

        public bool Errored { get; private set; }

        public void OnNext(ExtractionEvent value) => Events.Add(value);

        public void OnError(Exception error) => Errored = true;
    }
}
