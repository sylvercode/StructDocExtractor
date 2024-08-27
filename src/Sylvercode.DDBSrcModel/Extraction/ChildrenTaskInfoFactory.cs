using Sylvercode.DDBSrcModel.Model;
using Sylvercode.DDBSrcModel.Model.Init;

namespace Sylvercode.DDBSrcModel.Extraction;

public class ChildrenTaskInfoFactory : IChildrenTaskInfoFactory
{
    public IChildrenTaskInfo NewChildrenTaskInfo(
        ExtractionTask task,
        IEnumerable<object>? childrenData)
    {
        ISrcNode? resultNode = task.TaskResult?.SrcNode;
        if (resultNode is ISrcNodeHolderInitializer)
            return new NodeHolderChildrenTaskInfo(task, childrenData);

        if (resultNode is null
            && HasParentNodeHolder(task))
            return new ProxyChildrenTaskInfo(task, childrenData);

        return new ChildrenTaskInfo(task, childrenData);
    }

    private static bool HasParentNodeHolder(ExtractionTask task)
    {
        ExtractionTask? parentTask = task.ParentTaskInfo?.ParentTask;
        while (parentTask is not null)
        {
            if (parentTask.TaskResult?.SrcNode is ISrcNodeHolderInitializer)
                return true;
            parentTask = parentTask.ParentTaskInfo?.ParentTask;
        }

        return false;
    }

    public static ChildrenTaskInfoFactory Default { get; } = new();
}
