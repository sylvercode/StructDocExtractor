using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Options;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Tests.Mocks;
using Sylvercode.StructDocExtractor.Tests.Stubs;

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

    [Fact]
    public void MatchingExtractor_RouteToExtractor()
    {
        // Given
        RouterExtractorList<string> extractors = [];
        extractors.Add(new RouterExtractorSelectorMock(), new ThrowExtractorMock());
        extractors.Add(new RouterExtractorSelectorMock("ToUpper"), new ToUpperExtractorMock());
        extractors.Add(new RouterExtractorSelectorMock("ToUpper"), new ThrowExtractorMock());
        RouterExtractor<string> router = new(extractors, Options.Create(new RouterExtractor<string>.RouterExtractorOptions()));
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
        RouterExtractorList<string> extractors = [];
        extractors.Add(new RouterExtractorSelectorMock(), new ThrowExtractorMock());
        extractors.Add(new RouterExtractorSelectorMock("ToUpper"), new ToUpperExtractorMock());
        extractors.Add(new RouterExtractorSelectorMock("ToUpper"), new ThrowExtractorMock());
        RouterExtractor<string> router = new(extractors, Options.Create(new RouterExtractor<string>.RouterExtractorOptions()));
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
        RouterExtractorList<string> extractors = [];
        extractors.Add(new RouterExtractorSelectorMock(), new ThrowExtractorMock());
        extractors.Add(new RouterExtractorSelectorMock("ToUpper"), new ToUpperExtractorMock());
        extractors.Add(new RouterExtractorSelectorMock("ToUpper"), new ThrowExtractorMock());
        RouterExtractor<string> router = new(extractors, Options.Create(
            new RouterExtractor<string>.RouterExtractorOptions() { NoExtractorAsError = true }));
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
