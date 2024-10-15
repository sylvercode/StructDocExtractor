using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.Extraction.PreviewProvider;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

public class HtmlDataPreviewProvider : IDataPreviewProvider<IElement>
{
    public const string NullPreview = "<<null>>";

    public string GetPreview(IElement? data)
    {
        if (data is null)
            return NullPreview;

        string textContentPreview = data.TextContent.Length <= 50
            ? data.TextContent
            : data.TextContent[..50];

        return $"<{data.TagName}> {textContentPreview}";
    }
}
