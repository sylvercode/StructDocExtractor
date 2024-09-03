using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Extraction;

public class ExtractionTaskResult(TaskResultType resultType, IStructDocNode? srcNode = null, object? dataSelectable = null, object? nodeFactoryProvider = null)
{
    public ExtractionTaskResult(IProcessTaskResult p) : this(p.ResultType, p.SrcNode, p.DataSelectable, p.NodeFactoryProvider) { }
    public TaskResultType ResultType => resultType;
    public IStructDocNode? SrcNode => srcNode;
    public object? DataSelectable => dataSelectable;
    public object? NodeFactoryProvider => nodeFactoryProvider;
}
