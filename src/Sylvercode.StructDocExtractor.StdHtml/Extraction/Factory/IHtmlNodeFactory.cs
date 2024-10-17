using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.StdHtml.Extraction.Factory;

public interface IHtmlNodeFactory<TDataDiscriminator> : IStructDocNodeFactory<TDataDiscriminator, HtmlNodeDiscriminator>
{
    List<NodeScoreCriteriaSet<HtmlNodeDiscriminator>>? DefaultSelector { get; }
}
