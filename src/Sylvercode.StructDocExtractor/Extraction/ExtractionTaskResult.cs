using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Extraction;

public class ExtractionTaskResult(TaskResultType resultType, IStructDocNode? srcNode = null, object? dataDiscriminator = null, object? nodeFactoryProvider = null)
{
    public ExtractionTaskResult(IProcessTaskResult p) : this(p.ResultType, p.SrcNode, p.dataDiscriminator, p.NodeFactoryProvider) { }
    public TaskResultType ResultType => resultType;
    public IStructDocNode? SrcNode => srcNode;
    public object? DataDiscriminator => dataDiscriminator;
    public object? NodeFactoryProvider => nodeFactoryProvider;
}
