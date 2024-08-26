using Sylvercode.DDBSrcModel.Model;

namespace Sylvercode.DDBSrcModel.Extraction;

public class ExtractionTaskResult(ISrcNode? srcNode, object? dataSelectable, object? nodeFactoryProvider)
{
    public ExtractionTaskResult(IProcessTaskResult p) : this(p.SrcNode, p.DataSelectable, p.NodeFactoryProvider) { }
    public ISrcNode? SrcNode => srcNode;
    public object? DataSelectable => dataSelectable;
    public object? NodeFactoryProvider => nodeFactoryProvider;
}
