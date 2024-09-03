using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Model.Utils;
using Sylvercode.StructDocExtractor.StructDataStack;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
public class HtmlExtractionStack<TExtractionData>(
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
    IStructDocNodeFactoryProvider<TExtractionData, HtmlNodeDiscriminator> defaultSrcNodeFactoryProvider)
    : IStructDocNodeFactoryProviderStack<TExtractionData, HtmlNodeDiscriminator>
{
    public class Entry
    {
        public int Depth { get; private set; }
        public HtmlNodeDiscriminator? NodeDiscriminator { get; private set; }
        public IStructDocNode? SrcNode { get; private set; }
        public IStructDocNodeFactoryProvider<TExtractionData, HtmlNodeDiscriminator>? SrcNodeFactoryProvider { get; private set; }
    }

    private readonly Stack<Entry> m_Stack = [];

    public IStructDocNodeFactoryProvider<TExtractionData, HtmlNodeDiscriminator> DefaultSrcNodeFactoryProvider => defaultSrcNodeFactoryProvider;

    public IStructDataStack<HtmlNodeDiscriminator> AsStructDataStack()
    {
        return new BaseStructDataStack<HtmlNodeDiscriminator>(
            from entry in m_Stack
            let node = entry.NodeDiscriminator
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

    public IStructDocNodeFactoryProvider<TExtractionData, HtmlNodeDiscriminator> GetActiveSrcNodeFactoryProvider() =>
        m_Stack.First(entry => entry.SrcNodeFactoryProvider is not null)?.SrcNodeFactoryProvider ?? DefaultSrcNodeFactoryProvider;
}
