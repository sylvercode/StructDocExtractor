using Sylvercode.DDBSrcModel.Model;

namespace Sylvercode.DDBSrcModel.Extraction;

public class ExtractionTaskResult(TaskResultType resultType, ISrcNode? srcNode = null, object? dataSelectable = null, object? nodeFactoryProvider = null)
{
    public ExtractionTaskResult(IProcessTaskResult p) : this(p.ResultType, p.SrcNode, p.DataSelectable, p.NodeFactoryProvider) { }
    public TaskResultType ResultType => resultType;
    public ISrcNode? SrcNode => srcNode;
    public object? DataSelectable => dataSelectable;
    public object? NodeFactoryProvider => nodeFactoryProvider;
}
