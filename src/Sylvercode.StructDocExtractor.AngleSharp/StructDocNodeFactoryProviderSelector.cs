using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.AngleSharp;

public class StructDocNodeFactoryProviderSelector
{
    public interface ISelector
    {
        bool IsValid(IElement element);
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
        IStructDocNodeFactoryProvider<IElement, HtmlNodeDiscriminator>? defaultFactoryProvider)
        => _FactoryProviders.FirstOrDefault(entry => entry.Selector.IsValid(element))?.FactoryProvider
           ?? defaultFactoryProvider
           ?? throw new InvalidOperationException("No factory provider found");
}
