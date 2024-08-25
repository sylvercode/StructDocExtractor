using Sylvercode.DDBSrcModel.Factory;
using Sylvercode.DDBSrcModel.Model;
using Sylvercode.DDBSrcModel.Model.Utils;
using Sylvercode.DDBSrcModel.StructDocStack;

namespace Sylvercode.DDBSrcModel.Html;

public class HtmlExtractionStack(ISrcNodeFactoryProvider<HtmlNodeSelectable> defaulSrcNodeFactoryProvider)
    : ISrcNodeFactoryProviderStack<HtmlNodeSelectable>
{
    public class Entry
    {
        public int Depth { get; private set; }
        public HtmlNodeSelectable? NodeSelectable { get; private set; }
        public ISrcNode? SrcNode { get; private set; }
        public ISrcNodeFactoryProvider<HtmlNodeSelectable>? SrcNodeFactoryProvider { get; private set; }
    }

    private readonly Stack<Entry> m_Stack = [];

    public ISrcNodeFactoryProvider<HtmlNodeSelectable> DefaulSrcNodeFactoryProvider => defaulSrcNodeFactoryProvider;

    public IStructDataStack<HtmlNodeSelectable> AsStructDataStack()
    {
        return new BaseStructDataStack<HtmlNodeSelectable>(
            from entry in m_Stack
            let node = entry.NodeSelectable
            where node.HasValue
            select node.Value
        );
    }

    public ISrcNodeStack AsSrcNodeStack()
    {
        return new SrcNodeStack(
            from entry in m_Stack
            let node = entry.SrcNode
            where node is not null
            select node
        );
    }

    public ISrcNodeFactoryProvider<HtmlNodeSelectable> GetActiveSrcNodeFactoryProvider() =>
        m_Stack.First(entry => entry.SrcNodeFactoryProvider is not null)?.SrcNodeFactoryProvider ?? DefaulSrcNodeFactoryProvider;
}
