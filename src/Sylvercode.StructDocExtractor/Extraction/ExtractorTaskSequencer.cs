using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Extraction;

public partial class ExtractorTaskSequencer<TExtractionData, TDataDiscriminator>(
        IExtractorTaskSequencerHandler<TExtractionData, TDataDiscriminator> handler)
        where TExtractionData : notnull
{
    private readonly ILogger _logger = handler.CreateLogger<ExtractorTaskSequencer<TExtractionData, TDataDiscriminator>>();

    private readonly LinkedList<ExtractionTask> _pendingTacks = [];

    public bool HasPendingTask => _pendingTacks.First is not null;

    public event EventHandler? TaskResultSet;

    public ExtractionResult ProcessTasks()
    {
        ExtractionResult.ExtractionSummery summery = new();
        List<IStructDocNode> result = [];
        while (HasPendingTask)
        {
            ExtractionTask task = GetNextTask();
            IProcessTaskResult taskResult = ProcessTask(task);

            summery.CountTaskResult(taskResult.ResultType);

            if (task.ParentTaskInfo?.ParentTask is null)
            {
                if (taskResult.SrcNode is not null)
                    result.Add(taskResult.SrcNode);
                else if (taskResult.ResultType is TaskResultType.Success or TaskResultType.Warning)
                    LogRootTaskWithNoNodeResultOnSuccessOrWarning(handler.GetDataPreview((TExtractionData)task.ExtractionData));
            }
        }
        return new(summery, result);
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
        IProcessTaskResult<TExtractionData, TDataDiscriminator>? result = null;
        TaskContext<TExtractionData, TDataDiscriminator> taskContext = new(task, handler.DefaultNodeFactoryProvider);

        using (_logger.BeginScope(new List<KeyValuePair<string, object?>>(){
            new("TaskIndex", taskContext.TaskIndex),
            new("DataDiscriminatorStack", taskContext.GetStructDataStack().ToString())
        }))
        {
            try
            {
                result = handler.OnProcessTask(taskContext);

                LogProcessTaskResult(
                    result.ResultType,
                    result.DataDiscriminator?.ToString(),
                    result.SrcNode?.DebugName,
                    result.NodeFactoryProvider?.DebugName);

                task.SetResult(result, handler.ChildrenTaskInfoFactory);

                IReadOnlyList<ExtractionTask> subTask = task.ChildrenTaskInfo!.ChildrenTasks;
                LogChildrenTaskCount(subTask.Count);
                AddTasks(subTask, asNext: true);

                LogExtraTaskCount(subTask.Count);
                AddTasks(result.ExtraTasksExtractionData, asNext: false);
            }
            catch (Exception ex)
            {
                LogExceptionCatch(handler.ExtractorOption.ExceptionCatchLogLevel, ex);
                if (!handler.ExtractorOption.ContinueOnException)
                    throw;
                result = ProcessTaskResult.NewError<TExtractionData, TDataDiscriminator>();
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
        => AddTask(new ExtractionTask(data, logger: handler.CreateLogger<ExtractionTask>()), asNext);

    private void AddTask(ExtractionTask task, bool asNext = false)
    {
        if (TaskResultSet is not null)
            task.ResultSet += TaskResultSet;

        LogTaskAdded(handler.GetDataPreview((TExtractionData)task.ExtractionData),
                         asNext ? "Next" : "Last");
        if (asNext)
            _pendingTacks.AddFirst(task);
        else
            _pendingTacks.AddLast(task);
    }

    [LoggerMessage(
        Message = "Exception cath while processing task.")]
    private partial void LogExceptionCatch(LogLevel level, Exception ex);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Process task result: {ResultType}, DataDiscriminator: {DataDiscriminator}, SrcNode: {SrcNode}, NodeFactoryProvider: {NodeFactoryProvider}")]
    private partial void LogProcessTaskResult(TaskResultType resultType, string? dataDiscriminator, string? srcNode, string? nodeFactoryProvider);

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
