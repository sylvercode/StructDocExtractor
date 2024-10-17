using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

public class HtmlNodeDiscriminatorFactory : IDataDiscriminatorFactory<INode, HtmlNodeDiscriminator>
{
    public HtmlNodeDiscriminator CreateDataDiscriminator(INode data)
    {
        if (data.NodeType is NodeType.Text
            && !string.IsNullOrWhiteSpace(data.TextContent))
            return new(tagName: HtmlNodeDiscriminator.PlainTextTagName);

        if (data is not IElement element)
            return new();

        return new(element.Id ?? "", [.. element.ClassList], element.TagName);
    }
}
