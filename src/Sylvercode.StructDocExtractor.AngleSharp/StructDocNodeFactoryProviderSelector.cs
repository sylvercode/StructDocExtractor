using AngleSharp.Dom;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.StdHtml.Model;

namespace Sylvercode.StructDocExtractor.AngleSharp;

public class StructDocNodeFactoryProviderSelector
{
    public interface ISelector
    {
        bool IsValid(INode node, out INode rootExtraction);
    }

    private class Entry(ISelector selector, IStructDocNodeFactoryProvider<INode, HtmlNodeDiscriminator> factoryProvider)
    {
        public ISelector Selector { get; } = selector;
        public IStructDocNodeFactoryProvider<INode, HtmlNodeDiscriminator> FactoryProvider { get; } = factoryProvider;
    }

    private readonly List<Entry> _FactoryProviders = [];

    public void Add(ISelector selector, IStructDocNodeFactoryProvider<INode, HtmlNodeDiscriminator> factoryProvider)
        => _FactoryProviders.Add(new Entry(selector, factoryProvider));

    public IStructDocNodeFactoryProvider<INode, HtmlNodeDiscriminator> GetFactoryProvider(
        INode node,
        out INode rootExtraction)
    {
        foreach (var entry in _FactoryProviders)
        {
            if (entry.Selector.IsValid(node, out rootExtraction))
                return entry.FactoryProvider;
        }

        throw new InvalidOperationException("No factory provider found");
    }
}
