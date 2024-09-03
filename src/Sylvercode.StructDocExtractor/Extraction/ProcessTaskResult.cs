using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Extraction;

public class ProcessTaskResult<TExtractionData, TDataSelectable>(TaskResultType resultType, IStructDocNode? srcNode) : IProcessTaskResult<TExtractionData, TDataSelectable>
{
    public TaskResultType ResultType => resultType;

    public IStructDocNode? SrcNode => srcNode;

    public TDataSelectable? DataSelectable { get; set; }

    public ISrcNodeFactoryProvider<TExtractionData, TDataSelectable>? NodeFactoryProvider { get; set; }

    public List<TExtractionData> SubTasksExtractionData { get; } = [];
    IEnumerable<TExtractionData> IProcessTaskResult<TExtractionData, TDataSelectable>.SubTasksExtractionData => SubTasksExtractionData;

    public List<TExtractionData> ExtraTasksExtractionData { get; } = [];
    IEnumerable<TExtractionData> IProcessTaskResult<TExtractionData, TDataSelectable>.ExtraTasksExtractionData => ExtraTasksExtractionData;

    public ProcessTaskResult(IStructDocNode? srcNode = null) : this(TaskResultType.Success, srcNode) { }
}

public static class ProcessTaskResult
{
    public static ProcessTaskResult<TExtractionData, TDataSelectable> NewError<TExtractionData, TDataSelectable>()
        => new(TaskResultType.Error, null);

    public static ProcessTaskResult<TExtractionData, TDataSelectable> NewSkipped<TExtractionData, TDataSelectable>()
        => new(TaskResultType.Skipped, null);

    public static ProcessTaskResult<TExtractionData, TDataSelectable> NewErrorOrSkipped<TExtractionData, TDataSelectable>(bool asError)
        => asError ? NewError<TExtractionData, TDataSelectable>() : NewSkipped<TExtractionData, TDataSelectable>();
}
