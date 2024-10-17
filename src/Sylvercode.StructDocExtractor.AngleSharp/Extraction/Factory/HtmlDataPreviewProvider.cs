using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.Extraction.PreviewProvider;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

public class HtmlDataPreviewProvider : IDataPreviewProvider<INode>
{
    public const string NullPreview = "<<null>>";

    public string GetPreview(INode? data)
    {
        if (data is null)
            return NullPreview;

        string textContentPreview = data.TextContent.Length <= 50
            ? data.TextContent
            : data.TextContent[..50];

        if (data is not IElement element)
            return $"|PlainText| {textContentPreview}";

        return $"<{element.TagName}> {textContentPreview}";
    }
}
