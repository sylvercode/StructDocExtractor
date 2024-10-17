using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

public class HtmlNodeDiscriminatorFactory : IDataDiscriminatorFactory<INode, HtmlNodeDiscriminator>
{
    public HtmlNodeDiscriminator CreateDataDiscriminator(INode data)
    {
        if (data is not IElement element)
            return new(tagName: HtmlNodeDiscriminator.PlainTextTagName);

        return new(element.Id ?? "", [.. element.ClassList], element.TagName);
    }
}
