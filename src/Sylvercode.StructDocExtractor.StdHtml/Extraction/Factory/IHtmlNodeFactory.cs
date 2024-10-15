using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.StdHtml.Extraction.Factory;

public interface IHtmlNodeFactory<TDataDiscriminator> : IStructDocNodeFactory<TDataDiscriminator, HtmlNodeDiscriminator>
{
    HtmlNodeDiscriminator[]? DefaultSelector { get; }
}
