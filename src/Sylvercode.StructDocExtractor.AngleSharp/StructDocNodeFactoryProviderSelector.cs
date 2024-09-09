using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.AngleSharp;

public class StructDocNodeFactoryProviderSelector
{
    public interface ISelector
    {
        bool IsValid(IElement element, out IElement rootExtraction);
    }

    private class Entry(ISelector selector, IStructDocNodeFactoryProvider<IElement, HtmlNodeDiscriminator> factoryProvider)
    {
        public ISelector Selector { get; } = selector;
        public IStructDocNodeFactoryProvider<IElement, HtmlNodeDiscriminator> FactoryProvider { get; } = factoryProvider;
    }

    private readonly List<Entry> _FactoryProviders = [];

    public void Add(ISelector selector, IStructDocNodeFactoryProvider<IElement, HtmlNodeDiscriminator> factoryProvider)
        => _FactoryProviders.Add(new Entry(selector, factoryProvider));

    public IStructDocNodeFactoryProvider<IElement, HtmlNodeDiscriminator> GetFactoryProvider(
        IElement element,
        out IElement rootExtraction)
    {
        foreach (var entry in _FactoryProviders)
        {
            if (entry.Selector.IsValid(element, out rootExtraction))
                return entry.FactoryProvider;
        }

        throw new InvalidOperationException("No factory provider found");
    }
}
