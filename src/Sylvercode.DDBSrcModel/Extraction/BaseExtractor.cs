using Sylvercode.DDBSrcModel.Extraction.Factory;
using Sylvercode.DDBSrcModel.Model;

namespace Sylvercode.DDBSrcModel.Extraction;

public abstract partial class BaseExtractor<TExtractionData, TDataSelectable>(
        ISrcNodeFactoryProvider<TExtractionData, TDataSelectable> defaultNodeFactoryProvider)
        where TExtractionData : notnull
{
    private readonly LinkedList<ExtractionTask> _pendingTacks = [];

    public bool HasPendingTask => _pendingTacks.First is not null;

    public IEnumerable<ISrcNode> ExtractAll()
    {
        LinkedList<ISrcNode> result = [];
        while (HasPendingTask)
        {
            ExtractionTask task = ProcessNextTask();

            if (task.TaskResult?.SrcNode?.IsRoot ?? false)
                result.AddLast(task.TaskResult.SrcNode);
        }
        return result;
    }

    private ExtractionTask GetNextTask()
    {
        var firstListNode = _pendingTacks.First
                            ?? throw new InvalidOperationException("No task to process");

        _pendingTacks.Remove(firstListNode);

        return firstListNode.Value;
    }

    private ExtractionTask ProcessNextTask()
    {
        ExtractionTask curTask = GetNextTask();

        IProcessTaskResult<TExtractionData, TDataSelectable> result =
            ProcessTask(new TaskContext(curTask, defaultNodeFactoryProvider));

        curTask.ProcessResult(result);

        IReadOnlyList<ExtractionTask> subTask = curTask.ChildrenTaskInfo!.ChildrenTasks;
        AddTasks(subTask, asNext: true);
        AddTasks(result.ExtraTasksExtractionData, asNext: false);

        return curTask;
    }

    private void AddTasks(IEnumerable<ExtractionTask> extractionDatas, bool asNext = false)
    {
        var iterable = asNext ? extractionDatas.Reverse() : extractionDatas;
        foreach (var data in iterable)
            AddTask(data, asNext);
    }

    public void AddTasks(IEnumerable<TExtractionData> extractionDatas, bool asNext = false)
    {
        var iterable = asNext ? extractionDatas.Reverse() : extractionDatas;
        foreach (var data in iterable)
            AddTask(data, asNext);
    }

    public void AddTask(TExtractionData data, bool asNext = false)
        => AddTask(new ExtractionTask(data), asNext);

    private void AddTask(ExtractionTask task, bool asNext = false)
    {
        if (asNext)
            _pendingTacks.AddFirst(task);
        else
            _pendingTacks.AddLast(task);
    }

    protected virtual IProcessTaskResult<TExtractionData, TDataSelectable> ProcessTask(TaskContext taskContext)
    {
        ISrcNodeFactoryProvider<TExtractionData, TDataSelectable> factoryProvider =
            taskContext.GetNodeFactoryProvider();

        TDataSelectable selectable = GetSelectable(taskContext);

        ISrcNodeFactory<TExtractionData, TDataSelectable>? factory = factoryProvider.GetFactoryForStack(taskContext.GetStructDataStack(selectable));
        IProcessTaskResult<TExtractionData, TDataSelectable> result = factory.NewNode(taskContext.ExtractionData);

        return result;
    }

    protected virtual TDataSelectable GetSelectable(TaskContext taskContext)
    {
        throw new NotImplementedException();
    }
}
