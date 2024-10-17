using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.StdHtml.Model;
using Sylvercode.StructDocExtractor.StructDataStack.Score;

namespace Sylvercode.StructDocExtractor.StdHtml.Extraction.Factory;

public class HtmlNodeFactoryProvider<TExtractionData> : StructDocNodeFactoryProvider<TExtractionData, HtmlNodeDiscriminator>
{
    public void AddFactory(IHtmlNodeFactory<TExtractionData> factory)
    {
        if (factory.DefaultSelector is null)
            throw new InvalidOperationException("No selector set in factory");
        AddFactory(factory, new StackScoreCalculator<HtmlNodeDiscriminator>(factory.DefaultSelector));
    }
}
