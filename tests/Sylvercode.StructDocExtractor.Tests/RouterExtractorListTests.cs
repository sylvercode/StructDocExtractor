using System.Diagnostics.CodeAnalysis;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Tests.Mocks;

namespace Sylvercode.StructDocExtractor.Tests;

public class RouterExtractorListTests_Select
{
    private class ExtractorMock : IExtractor<object>
    {
        public static ExtractorMock Instance1 { get; } = new ExtractorMock();
        public static ExtractorMock Instance2 { get; } = new ExtractorMock();
        public static ExtractorMock Instance3 { get; } = new ExtractorMock();

        public ExtractionResult Extract([DisallowNull] object data, IObserver<ExtractionTask>? observer = null)
            => throw new NotImplementedException();
    }

    [Fact]
    public void ExtractorExists_Found()
    {
        // Given
        RouterExtractorList<object> entries = [];
        entries.Add(new RouterExtractorSelectorMock(), ExtractorMock.Instance1);
        entries.Add(new RouterExtractorSelectorMock(), ExtractorMock.Instance2);
        entries.Add(new RouterExtractorSelectorMock(result: true), ExtractorMock.Instance3);
        object data = new();

        // When
        RouterExtractorList<object>.SelectionResult? result = entries.Select(data);

        // Then
        Assert.NotNull(result);
        Assert.Same(ExtractorMock.Instance3, result.Extractor);
        Assert.Same(data, result.RootData);
    }

    [Fact]
    public void ExtractorNotExists_NotFound()
    {
        // Given
        RouterExtractorList<object> entries = [];
        entries.Add(new RouterExtractorSelectorMock(), ExtractorMock.Instance1);
        entries.Add(new RouterExtractorSelectorMock(), ExtractorMock.Instance2);
        entries.Add(new RouterExtractorSelectorMock(), ExtractorMock.Instance3);

        // When
        var result = entries.Select(new object());

        // Then
        Assert.Null(result);
    }
}
