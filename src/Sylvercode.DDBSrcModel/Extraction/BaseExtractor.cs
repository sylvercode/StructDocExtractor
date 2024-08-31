using Microsoft.Extensions.Logging;
using Sylvercode.DDBSrcModel.Extraction.Factory;
using Sylvercode.DDBSrcModel.Model;

namespace Sylvercode.DDBSrcModel.Extraction;

public abstract partial class BaseExtractor<TExtractionData, TDataSelectable>(
        ISrcNodeFactoryProvider<TExtractionData, TDataSelectable> defaultNodeFactoryProvider,
        ILogger logger,
        IChildrenTaskInfoFactory? childrenTaskInfoFactory = null,
        BaseExtractorOption option = default)
        where TExtractionData : notnull
{
    private ILogger Logger => logger;
    private readonly LinkedList<ExtractionTask> _pendingTacks = [];
    private readonly IChildrenTaskInfoFactory _childrenTaskInfoFactory = childrenTaskInfoFactory ?? ChildrenTaskInfoFactory.Default;

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

        curTask.SetResult(result, _childrenTaskInfoFactory);

        IReadOnlyList<ExtractionTask> subTask = curTask.ChildrenTaskInfo!.ChildrenTasks;
        AddTasks(subTask, asNext: true);

        if (result is not null)
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

    protected virtual IProcessTaskResult<TExtractionData, TDataSelectable>
        ProcessTask(TaskContext taskContext)
    {
        ISrcNodeFactoryProvider<TExtractionData, TDataSelectable> factoryProvider =
            taskContext.GetNodeFactoryProvider();

        TDataSelectable selectable = GetDataSelectable(taskContext);

        ISrcNodeFactory<TExtractionData, TDataSelectable>? factory =
            factoryProvider.GetFactoryForStack(taskContext.GetStructDataStack(selectable));

        if (factory is null)
        {
            if (Logger.IsEnabled(option.MissingNodeFactoryLogLevel))
                LogNoNodeFactoryFound(option.MissingNodeFactoryLogLevel, taskContext.GetStructDataStack(selectable).ToString());
            return ProcessTaskResult<TExtractionData, TDataSelectable>.ErrorOrSkipped(option.MissingNodeFactoryAsError);
        }

        return factory.NewNode(taskContext.ExtractionData);
    }

    protected virtual TDataSelectable GetDataSelectable(TaskContext taskContext)
    {
        throw new NotImplementedException();
    }

    [LoggerMessage(
       Message = "No factory found for `{dataStack}`",
       SkipEnabledCheck = true)]
    private partial void LogNoNodeFactoryFound(LogLevel level, string? dataStack);
}
