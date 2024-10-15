using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;

public class HtmlNodeDiscriminatorFactory : IDataDiscriminatorFactory<IElement, HtmlNodeDiscriminator>
{
    public HtmlNodeDiscriminator CreateDataDiscriminator(IElement data) =>
        new(data.Id ?? "", [.. data.ClassList], data.TagName);
}
