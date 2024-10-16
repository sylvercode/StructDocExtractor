using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Metadatas;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Extraction;

public class ProcessTaskResult<TExtractionData, TDataDiscriminator>(TaskResultType resultType, IStructDocNode? srcNode)
    : IProcessTaskResult<TExtractionData, TDataDiscriminator>
{
    public TaskResultType ResultType => resultType;

    public IStructDocNode? SrcNode => srcNode;

    public TDataDiscriminator? DataDiscriminator { get; set; }

    public IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator>? NodeFactoryProvider { get; set; }

    public MetadataDictionary Metadatas { get; set; } = [];

    public List<TExtractionData> SubTasksExtractionData { get; set; } = [];
    IEnumerable<TExtractionData> IProcessTaskResult<TExtractionData, TDataDiscriminator>.SubTasksExtractionData => SubTasksExtractionData;

    public List<TExtractionData> ExtraTasksExtractionData { get; set; } = [];
    IEnumerable<TExtractionData> IProcessTaskResult<TExtractionData, TDataDiscriminator>.ExtraTasksExtractionData => ExtraTasksExtractionData;

    public ProcessTaskResult(IStructDocNode? srcNode = null) : this(TaskResultType.Success, srcNode) { }
}

public static class ProcessTaskResult
{
    public static ProcessTaskResult<TExtractionData, TDataDiscriminator> NewError<TExtractionData, TDataDiscriminator>()
        => new(TaskResultType.Error, null);

    public static ProcessTaskResult<TExtractionData, TDataDiscriminator> NewSkipped<TExtractionData, TDataDiscriminator>()
        => new(TaskResultType.Skipped, null);

    public static ProcessTaskResult<TExtractionData, TDataDiscriminator> NewErrorOrSkipped<TExtractionData, TDataDiscriminator>(bool asError)
        => asError ? NewError<TExtractionData, TDataDiscriminator>() : NewSkipped<TExtractionData, TDataDiscriminator>();
}
