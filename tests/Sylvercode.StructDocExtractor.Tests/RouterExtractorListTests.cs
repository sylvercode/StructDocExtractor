using System.Diagnostics.CodeAnalysis;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.Tests.Mocks;

namespace Sylvercode.StructDocExtractor.Tests;

public class RouterExtractorListTests_Select
{
    private class ExtractorMock : IExtractor<string>
    {
        public static ExtractorMock Instance1 { get; } = new ExtractorMock();
        public static ExtractorMock Instance2 { get; } = new ExtractorMock();
        public static ExtractorMock Instance3 { get; } = new ExtractorMock();

        public ExtractionResult Extract([DisallowNull] string data, IObserver<ExtractionTask>? observer = null)
            => throw new NotImplementedException();
    }

    [Fact]
    public void ExtractorExists_Found()
    {
        // Given
        RouterExtractorList<string> entries = [];
        entries.Add(new RouterExtractorSelectorMock(), ExtractorMock.Instance1);
        entries.Add(new RouterExtractorSelectorMock(), ExtractorMock.Instance2);
        entries.Add(new RouterExtractorSelectorMock("key"), ExtractorMock.Instance3);

        // When
        RouterExtractorList<string>.SelectionResult? result = entries.Select("key:value");

        // Then
        Assert.NotNull(result);
        Assert.Same(ExtractorMock.Instance3, result.Extractor);
        Assert.Equal("value", result.RootData);
    }

    [Fact]
    public void ExtractorNotExists_NotFound()
    {
        // Given
        RouterExtractorList<string> entries = [];
        entries.Add(new RouterExtractorSelectorMock(), ExtractorMock.Instance1);
        entries.Add(new RouterExtractorSelectorMock(), ExtractorMock.Instance2);
        entries.Add(new RouterExtractorSelectorMock(), ExtractorMock.Instance3);

        // When
        var result = entries.Select("key:value");

        // Then
        Assert.Null(result);
    }
}
