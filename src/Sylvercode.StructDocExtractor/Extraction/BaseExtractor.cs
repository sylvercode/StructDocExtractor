using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Extraction;

public abstract partial class BaseExtractor<TExtractionData, TDataSelectable>(
        ISrcNodeFactoryProvider<TExtractionData, TDataSelectable> defaultNodeFactoryProvider,
        IChildrenTaskInfoFactory? childrenTaskInfoFactory = null,
        IDataPreviewProvider<TExtractionData>? dataPreviewProvider = null,
        BaseExtractorOption option = default,
        ILoggerFactory? loggerFactory = null)
        where TExtractionData : notnull
{
    private readonly IChildrenTaskInfoFactory _childrenTaskInfoFactory = childrenTaskInfoFactory
                                                                         ?? ChildrenTaskInfoFactory.Default;
    private readonly IDataPreviewProvider<TExtractionData> _dataPreviewProvider = dataPreviewProvider
                                                                                  ?? new ToStringPreviewProvider<TExtractionData>();

    private readonly ILoggerFactory _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
    private readonly ILogger _logger = loggerFactory is not null ? loggerFactory.CreateLogger<BaseExtractor<TExtractionData, TDataSelectable>>()
                                                                 : NullLogger.Instance;

    private readonly LinkedList<ExtractionTask> _pendingTacks = [];
    private readonly ISrcNodeFactoryProvider<TExtractionData, TDataSelectable> defaultNodeFactoryProvider = defaultNodeFactoryProvider;

    public bool HasPendingTask => _pendingTacks.First is not null;

    public IEnumerable<IStructDocNode> ExtractAll()
    {
        ExtractionResult.ExtractionSummery summery = new();
        LinkedList<IStructDocNode> result = [];
        while (HasPendingTask)
        {
            ExtractionTask task = GetNextTask();
            IProcessTaskResult taskResult = ProcessTask(task);

            summery.CountTaskResult(taskResult.ResultType);

            if (task.ParentTaskInfo?.ParentTask is null)
            {
                if (taskResult.SrcNode is not null)
                    result.AddLast(taskResult.SrcNode);
                else if (taskResult.ResultType is TaskResultType.Success or TaskResultType.Warning)
                    LogRootTaskWithNoNodeResultOnSuccessOrWarning(_dataPreviewProvider.GetPreview((TExtractionData)task.ExtractionData));
            }
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

    private IProcessTaskResult ProcessTask(ExtractionTask task)
    {
        IProcessTaskResult<TExtractionData, TDataSelectable>? result = null;
        TaskContext taskContext = new(task, defaultNodeFactoryProvider);

        using (_logger.BeginScope(new List<KeyValuePair<string, object?>>(){
            new("TaskIndex", taskContext.TaskIndex),
            new("DataSelectableStack", taskContext.GetStructDataStack().ToString())
        }))
        {
            try
            {
                result = ProcessTask(taskContext);

                LogProcessTaskResult(
                    result.ResultType,
                    result.DataSelectable?.ToString(),
                    result.SrcNode?.DebugName,
                    result.NodeFactoryProvider?.DebugName);

                task.SetResult(result, _childrenTaskInfoFactory);

                IReadOnlyList<ExtractionTask> subTask = task.ChildrenTaskInfo!.ChildrenTasks;
                LogChildrenTaskCount(subTask.Count);
                AddTasks(subTask, asNext: true);

                LogExtraTaskCount(subTask.Count);
                AddTasks(result.ExtraTasksExtractionData, asNext: false);
            }
            catch (Exception ex)
            {
                LogExceptionCatch(option.ExceptionCatchLogLevel, ex);
                if (!option.ContinueOnException)
                    throw;
                result = ProcessTaskResult.NewError<TExtractionData, TDataSelectable>();
            }
        }

        return result;
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
        => AddTask(new ExtractionTask(data, logger: _loggerFactory.CreateLogger<ExtractionTask>()), asNext);

    private void AddTask(ExtractionTask task, bool asNext = false)
    {
        LogTaskAdded(_dataPreviewProvider.GetPreview((TExtractionData)task.ExtractionData),
                     asNext ? "Next" : "Last");
        if (asNext)
            _pendingTacks.AddFirst(task);
        else
            _pendingTacks.AddLast(task);
    }

    protected virtual IProcessTaskResult<TExtractionData, TDataSelectable>
        ProcessTask(TaskContext taskContext)
    {
        LogTraceProcessBegin();

        ISrcNodeFactoryProvider<TExtractionData, TDataSelectable> factoryProvider =
            taskContext.GetNodeFactoryProvider();
        LogFactoryProviderInUse(factoryProvider.DebugName);

        TDataSelectable selectable = GetDataSelectable(taskContext);
        LogDataSelectableGot(selectable?.ToString());

        ISrcNodeFactory<TExtractionData, TDataSelectable>? factory =
            factoryProvider.GetFactoryForStack(taskContext.GetStructDataStack(selectable));
        LogNoNodeFactoryFound(option.MissingNodeFactoryLogLevel, selectable?.ToString());
        if (factory is null)
            return ProcessTaskResult.NewErrorOrSkipped<TExtractionData, TDataSelectable>(option.MissingNodeFactoryAsError);

        return factory.NewNode(taskContext.ExtractionData);
    }

    protected virtual TDataSelectable GetDataSelectable(TaskContext taskContext)
    {
        // TODO: Implement this method
        throw new NotImplementedException();
    }

    [LoggerMessage(
       Message = "No factory found for `{dataSelectable}`.")]
    private partial void LogNoNodeFactoryFound(LogLevel level, string? dataSelectable);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Data selectable `{DataSelectable}`.")]
    private partial void LogDataSelectableGot(string? dataSelectable);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Factory provider `{FactoryProviderName}` in use.")]
    private partial void LogFactoryProviderInUse(string factoryProviderName);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Begin process task.")]
    private partial void LogTraceProcessBegin();

    [LoggerMessage(
        Message = "Exception cath while processing task.")]
    private partial void LogExceptionCatch(LogLevel level, Exception ex);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Process task result: {ResultType}, DataSelectable: {DataSelectable}, SrcNode: {SrcNode}, NodeFactoryProvider: {NodeFactoryProvider}")]
    private partial void LogProcessTaskResult(TaskResultType resultType, string? dataSelectable, string? srcNode, string? nodeFactoryProvider);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Children task count: {Count}")]
    private partial void LogChildrenTaskCount(int count);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Extra task count: {Count}")]
    private partial void LogExtraTaskCount(int count);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Task added for `{DataPreview}` as {NextOrLast}")]
    private partial void LogTaskAdded(string dataPreview, string nextOrLast);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "No node result on success or warning for `{DataPreview}`.")]
    private partial void LogRootTaskWithNoNodeResultOnSuccessOrWarning(string dataPreview);
}
