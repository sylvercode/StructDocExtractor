using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Model.Utils;
using Sylvercode.StructDocExtractor.StructDataStack;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public class HtmlExtractionStack<TExtractionData>(
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    ISrcNodeFactoryProvider<TExtractionData, HtmlNodeSelectable> defaultSrcNodeFactoryProvider)
    : ISrcNodeFactoryProviderStack<TExtractionData, HtmlNodeSelectable>
{
    public class Entry
    {
        public int Depth { get; private set; }
        public HtmlNodeSelectable? NodeSelectable { get; private set; }
        public IStructDocNode? SrcNode { get; private set; }
        public ISrcNodeFactoryProvider<TExtractionData, HtmlNodeSelectable>? SrcNodeFactoryProvider { get; private set; }
    }

    private readonly Stack<Entry> m_Stack = [];

    public ISrcNodeFactoryProvider<TExtractionData, HtmlNodeSelectable> DefaultSrcNodeFactoryProvider => defaultSrcNodeFactoryProvider;

    public IStructDataStack<HtmlNodeSelectable> AsStructDataStack()
    {
        return new BaseStructDataStack<HtmlNodeSelectable>(
            from entry in m_Stack
            let node = entry.NodeSelectable
            where node.HasValue
            select node.Value
        );
    }

    public IStructDocNodeStack AsSrcNodeStack()
    {
        return new StructDocNodeStack(
            from entry in m_Stack
            let node = entry.SrcNode
            where node is not null
            select node
        );
    }

    public ISrcNodeFactoryProvider<TExtractionData, HtmlNodeSelectable> GetActiveSrcNodeFactoryProvider() =>
        m_Stack.First(entry => entry.SrcNodeFactoryProvider is not null)?.SrcNodeFactoryProvider ?? DefaultSrcNodeFactoryProvider;
}
