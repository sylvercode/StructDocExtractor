using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using Sylvercode.StructDocExtractor.AngleSharp;
using Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;
using Sylvercode.StructDocExtractor.Extraction;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.Tests;

public class AngleSharpNoOpNodeFactoryTests
{
    private static INode NewTextNode(string text)
    {
        HtmlParser parser = new();
        var document = parser.ParseDocument("<div></div>");
        return document.CreateTextNode(text);
    }

    private static INode ParseFirstNode(string html)
    {
        HtmlParser parser = new();
        var document = parser.ParseDocument(html);

        return document.Body?.FirstChild ?? throw new InvalidOperationException("No first node in parsed body.");
    }

    private static Extractor<INode, HtmlNodeDiscriminator> NewExtractor(bool missingNodeFactoryAsError = true)
    {
        return new(
            new AngleSharpNodeFactoryProvider(selector: null),
            new ExtractorOption() { MissingNodeFactoryAsError = missingNodeFactoryAsError },
            new HtmlNodeDiscriminatorFactory());
    }

    [Fact]
    public void WhitespaceTextNode_EmptyDiscriminator_IsSkippedWithoutError()
    {
        // Given
        INode node = NewTextNode("   ");
        Extractor<INode, HtmlNodeDiscriminator> extractor = NewExtractor();

        // When
        ExtractionResult result = extractor.Extract(node);

        // Then
        Assert.Equal(1, result.Summery.ProcessedTaskCount);
        Assert.Equal(0, result.Summery.ErrorTaskCount);
        Assert.Equal(1, result.Summery.SkippedTaskCount);
        Assert.Empty(result.StructDocNodes);
    }

    [Fact]
    public void UnknownElementNode_NonEmptyDiscriminator_RemainsMissingFactoryError()
    {
        // Given
        INode node = ParseFirstNode("<custom-tag></custom-tag>");
        Extractor<INode, HtmlNodeDiscriminator> extractor = NewExtractor();

        // When
        ExtractionResult result = extractor.Extract(node);

        // Then
        Assert.Equal(1, result.Summery.ProcessedTaskCount);
        Assert.Equal(1, result.Summery.ErrorTaskCount);
        Assert.Equal(0, result.Summery.SkippedTaskCount);
        Assert.Empty(result.StructDocNodes);
    }

    [Fact]
    public void BlockquoteElementNode_IsExtractedAsHtmlAside()
    {
        // Given
        INode node = ParseFirstNode("<blockquote>hello</blockquote>");
        Extractor<INode, HtmlNodeDiscriminator> extractor = NewExtractor();

        // When
        ExtractionResult result = extractor.Extract(node);

        // Then
        Assert.Equal(2, result.Summery.ProcessedTaskCount);
        Assert.Equal(0, result.Summery.ErrorTaskCount);
        Assert.Equal(0, result.Summery.SkippedTaskCount);
        HtmlAside aside = Assert.IsType<HtmlAside>(Assert.Single(result.StructDocNodes));
        Assert.Single(aside.Content);
        Assert.IsType<PlainTextNode>(aside.Content[0]);
    }
}
