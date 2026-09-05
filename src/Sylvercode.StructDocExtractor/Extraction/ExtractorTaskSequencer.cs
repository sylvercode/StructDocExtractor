using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Model.Init;

namespace Sylvercode.StructDocExtractor.Extraction;

/// <summary>Sequences and processes extraction tasks in order, managing the pending-task queue and notifying observers as each task completes.</summary>
/// <typeparam name="TExtractionData">The type of source data element being extracted.</typeparam>
/// <typeparam name="TDataDiscriminator">The discriminator type used to select node factories.</typeparam>
/// <param name="handler">The handler responsible for processing each individual extraction task.</param>
/// <remarks>
/// Uses a <see cref="LinkedList{T}"/> to allow child tasks to be prepended (depth-first) or appended (breadth-first).
/// Implements <see cref="IObservable{T}"/> so callers can subscribe to task-completion events.
/// </remarks>
public partial class ExtractorTaskSequencer<TExtractionData, TDataDiscriminator>
    (IExtractorTaskSequencerHandler<TExtractionData, TDataDiscriminator> handler)
    : IObservable<ExtractionTask>
{
    private readonly ILogger _logger = handler.CreateLogger<ExtractorTaskSequencer<TExtractionData, TDataDiscriminator>>();

    private readonly LinkedList<ExtractionTask> _pendingTacks = [];

    /// <summary>Gets a value indicating whether there are tasks remaining in the pending-task queue.</summary>
    public bool HasPendingTask => _pendingTacks.First is not null;

    /// <summary>Drains the task queue, processes each task via the handler, and returns the aggregated extraction result.</summary>
    /// <returns>An <see cref="ExtractionResult"/> containing all produced nodes, merged metadata, and task statistics.</returns>
    public ExtractionResult ProcessTasks()
    {
        ExtractionResult result = new();
        while (HasPendingTask)
        {
            ExtractionTask task = GetNextTask();
            IProcessTaskResult taskResult = ProcessTask(task);

            result.Summery.CountTaskResult(taskResult.ResultType);

            result.Metadatas.CopyMetadataFrom(taskResult.Metadatas, newOnly: false);

            if (task.ParentTaskInfo?.ParentTask is null)
            {
                if (taskResult.SrcNode is not null)
                {
                    if (!taskResult.SrcNode.IsRoot
                        && taskResult.SrcNode is IStructDocNodeInitializer nodeInitializer)
                        nodeInitializer.MakeARoot();
                    result.StructDocNodes.Add(taskResult.SrcNode);
                }
                else if (taskResult.ResultType is TaskResultType.Success or TaskResultType.Warning)
                    LogRootTaskWithNoNodeResultOnSuccessOrWarning(handler.GetDataPreview((TExtractionData)task.ExtractionData));
            }
        }

        OnCompleted();
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
        IProcessTaskResult<TExtractionData, TDataDiscriminator>? result = null;
        TaskContext<TExtractionData, TDataDiscriminator> taskContext = new(task, handler.DefaultNodeFactoryProvider);

        using var scope = _logger.BeginScope(new List<KeyValuePair<string, object?>>()
            {
                new("TaskIndex", taskContext.TaskIndex),
                new("DataDiscriminatorStack", taskContext.GetStructDataStack().ToString())
            });

        try
        {
            result = handler.OnProcessTask(taskContext);

            LogProcessTaskResult(
                result.ResultType,
                result.DataDiscriminator?.ToString(),
                result.SrcNode?.DebugName,
                result.NodeFactoryProvider?.DebugName);

            task.SetResult(result);

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

        return result;
    }

    private void AddTasks(IEnumerable<ExtractionTask> extractionDatas, bool asNext = false)
    {
        var iterable = asNext ? extractionDatas.Reverse() : extractionDatas;
        foreach (var data in iterable)
            AddTask(data, asNext);
    }

    /// <summary>Enqueues a sequence of typed data items as new extraction tasks.</summary>
    /// <param name="extractionDatas">The source data elements to enqueue.</param>
    /// <param name="asNext">When <see langword="true"/>, prepends each item so it is processed next (depth-first); otherwise appends (breadth-first).</param>
    public void AddTasks(IEnumerable<TExtractionData> extractionDatas, bool asNext = false)
    {
        var iterable = asNext ? extractionDatas.Reverse() : extractionDatas;
        foreach (var data in iterable)
        {
            if (data is not null)
                AddTask(data, asNext);
        }
    }

    /// <summary>Enqueues a single typed data item as a new extraction task.</summary>
    /// <param name="data">The source data element to enqueue.</param>
    /// <param name="asNext">When <see langword="true"/>, prepends the task so it is processed next (depth-first); otherwise appends (breadth-first).</param>
    public void AddTask([DisallowNull] TExtractionData data, bool asNext = false)
        => AddTask(new ExtractionTask(data, logger: handler.CreateLogger<ExtractionTask>()), asNext);

    private void AddTask(ExtractionTask task, bool asNext = false)
    {
        task.ResultSet += OnTaskResult;

        LogTaskAdded(handler.GetDataPreview((TExtractionData)task.ExtractionData),
                         asNext ? "Next" : "Last");
        if (asNext)
            _pendingTacks.AddFirst(task);
        else
            _pendingTacks.AddLast(task);
    }

    private void OnTaskResult(object? sender, EventArgs e)
    {
        if (sender is not ExtractionTask task)
            return;

        foreach (var observer in observers)
            observer.OnNext(task);
    }

    private void OnCompleted()
    {
        foreach (var observer in observers)
            observer.OnCompleted();
    }

    #region IObservable
    private readonly List<IObserver<ExtractionTask>> observers = [];
    /// <summary>Registers an observer to receive completed <see cref="ExtractionTask"/> notifications as each task result is set.</summary>
    /// <param name="observer">The observer to register.</param>
    /// <returns>An <see cref="IDisposable"/> that removes the observer when disposed.</returns>
    public IDisposable Subscribe(IObserver<ExtractionTask> observer)
    {
        if (!observers.Contains(observer))
            observers.Add(observer);
        return new Unsubscriber(observers, observer);
    }

    private sealed class Unsubscriber(List<IObserver<ExtractionTask>> observers, IObserver<ExtractionTask> observer) : IDisposable
    {
        private readonly List<IObserver<ExtractionTask>> _observers = observers;
        private readonly IObserver<ExtractionTask> _observer = observer;

        public void Dispose()
        {
            if (_observer != null && _observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
    #endregion

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
