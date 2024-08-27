using Sylvercode.DDBSrcModel.Model.Init;

namespace Sylvercode.DDBSrcModel.Extraction;

public class ChildrenTaskInfoFactory : IChildrenTaskInfoFactory
{
    public IChildrenTaskInfo NewChildrenTaskInfo(ExtractionTask task, IEnumerable<object>? childrenData)
    {
        if (task.TaskResult?.SrcNode is ISrcNodeHolderInitializer)
            return new NodeHolderChildrenTaskInfo(task, childrenData);

        return new ChildrenTaskInfo(task, childrenData);
    }

    public static ChildrenTaskInfoFactory Default { get; } = new();
}
