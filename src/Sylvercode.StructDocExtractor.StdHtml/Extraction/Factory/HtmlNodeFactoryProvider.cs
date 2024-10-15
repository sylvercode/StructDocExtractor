using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StdHtml.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.StdHtml.Extraction.Factory;

public class HtmlNodeFactoryProvider<TExtractionData> : StructDocNodeFactoryProvider<TExtractionData, HtmlNodeDiscriminator>
{
    public HtmlNodeFactoryProvider()
    {
        HtmlNodeDiscriminator t = new(styleClass: ["compendium-toc-full-text"]);
        AddFactory(null!, t.AsScoreCalculator());// TODO initialize
    }

    public void AddFactory(IHtmlNodeFactory<TExtractionData> factory)
    {
        HtmlNodeDiscriminator selector = factory.DefaultSelector ?? throw new InvalidOperationException("No selector set in factory");
        AddFactory(factory, selector.AsScoreCalculator());
    }

    public void AddFactory(IStructDocNodeFactory<TExtractionData, HtmlNodeDiscriminator> factory, HtmlNodeDiscriminator discriminator)
    {
        AddFactory(factory, discriminator.AsScoreCalculator());
    }
}
