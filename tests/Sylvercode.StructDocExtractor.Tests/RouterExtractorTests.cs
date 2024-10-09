using System.Diagnostics.CodeAnalysis;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Tests.Mocks;
using Sylvercode.StructDocExtractor.Tests.Stubs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Sylvercode.StructDocExtractor.Tests;

public class RouterExtractorTests
{
    public class ExtractionTaskObserverMock : IObserver<ExtractionTask>
    {
        public List<ExtractionTask> Tasks { get; } = [];

        public void OnCompleted() { }

        public void OnError(Exception error) { }

        public void OnNext(ExtractionTask value)
            => Tasks.Add(value);
    }
    private class ToUpperExtractorMock : IExtractor<string>
    {
        public ExtractionResult Extract([DisallowNull] string data, IObserver<ExtractionTask>? observer = null)
        {
            BasicSrcRootBlock node = new(data.ToUpperInvariant());

            ExtractionTask task = new(data);
            task.SetResult(new BasicProcessTaskResult(node), ChildrenTaskInfoFactory.Default);
            observer?.OnNext(task);

            ExtractionResult result = new();
            result.Summery.CountTaskResult(TaskResultType.Success);
            result.StructDocNodes.Add(node);
            return result;
        }
    }

    private class ThrowExtractorMock : IExtractor<string>
    {
        public ExtractionResult Extract([DisallowNull] string data, IObserver<ExtractionTask>? observer = null)
            => throw new NotImplementedException();
    }

    public static IHost GetDefaultHost(bool noExtractorAsError = false)
        => Host.CreateDefaultBuilder().ConfigureServices(services =>
            services
                .AddRouterExtractor<string>(b =>
            {
                b.AddExtractor<ThrowExtractorMock>(new RouterExtractorSelectorMock());
                b.AddExtractor<ToUpperExtractorMock>(new RouterExtractorSelectorMock("ToUpper"));
                b.AddExtractor<ThrowExtractorMock>(new RouterExtractorSelectorMock("ToUpper"));
                if (noExtractorAsError)
                    b.NoExtractorAsError();

            }))
            .Build();

    [Fact]
    public void MatchingExtractor_RouteToExtractor()
    {
        // Given
        IHost host = GetDefaultHost();
        var router = host.Services.GetRequiredService<IExtractor<string>>();
        ExtractionTaskObserverMock observer = new();

        // When
        ExtractionResult result = router.Extract("ToUpper:hello", observer);

        // Then
        Assert.Equal(1, result.Summery.ProcessedTaskCount);
        Assert.Equal(0, result.Summery.ErrorTaskCount);
        Assert.Equal(0, result.Summery.SkippedTaskCount);
        IStructDocNode node = Assert.Single(result.StructDocNodes);
        Assert.Equal("HELLO", node.Id);
        ExtractionTask task = Assert.Single(observer.Tasks);
        Assert.Equal("hello", task.ExtractionData);
    }

    [Fact]
    public void NoMatchingExtractor_Skipped()
    {
        // Given
        IHost host = GetDefaultHost();
        var router = host.Services.GetRequiredService<IExtractor<string>>();
        ExtractionTaskObserverMock observer = new();

        // When
        ExtractionResult result = router.Extract("ToLower:hello", observer);

        // Then
        Assert.Equal(1, result.Summery.ProcessedTaskCount);
        Assert.Equal(0, result.Summery.ErrorTaskCount);
        Assert.Equal(1, result.Summery.SkippedTaskCount);
        Assert.Empty(result.StructDocNodes);
        Assert.Empty(observer.Tasks);
    }

    [Fact]
    public void NoMatchingExtractor_Error()
    {
        // Given
        IHost host = GetDefaultHost(noExtractorAsError: true);
        var router = host.Services.GetRequiredService<IExtractor<string>>();
        ExtractionTaskObserverMock observer = new();

        // When
        ExtractionResult result = router.Extract("ToLower:hello", observer);

        // Then
        Assert.Equal(1, result.Summery.ProcessedTaskCount);
        Assert.Equal(1, result.Summery.ErrorTaskCount);
        Assert.Equal(0, result.Summery.SkippedTaskCount);
        Assert.Empty(result.StructDocNodes);
        Assert.Empty(observer.Tasks);
    }
}
