using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

/// <summary>Executes a queue of <see cref="SerializerTask"/> objects, driving the depth-first serialization of an entire document tree.</summary>
/// <remarks>
/// Transforms the recursive document tree into a flat, ordered stream of serialization calls.
/// For each task, it resolves a serializer via <see cref="ISerializerProvider"/>, calls
/// <see cref="SerializerTask.OnToProcess"/>, invokes the serializer, and — unless the serializer set
/// <see cref="NodeSerializationResult.ContentSerialized"/> — prepends child tasks to the linked list in
/// reverse order to maintain correct depth-first traversal while wiring sibling links for
/// before/between/after notifications.
/// </remarks>
public partial class StructDocSerializerExecutor
{
    private readonly LinkedList<SerializerTask> _pendingTasks;
    private readonly TextWriter _stream;
    private readonly ISerializerProvider _serializerProvider;

    private bool HasPendingTasks => _pendingTasks.Count > 0;

    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<StructDocSerializerExecutor> _logger;
    private readonly ILogger<SerializerTask> _loggerForTask;

    /// <summary>Initializes a new <see cref="StructDocSerializerExecutor"/> and seeds the task queue with a root task for <paramref name="rootData"/>.</summary>
    /// <param name="stream">The writer to which all serialized output is written.</param>
    /// <param name="rootData">The root node of the document tree to serialize.</param>
    /// <param name="serializerProvider">The provider used to resolve each node's serializer.</param>
    /// <param name="loggerFactory">An optional factory for creating loggers; uses a null logger if <see langword="null"/>.</param>
    public StructDocSerializerExecutor(
        TextWriter stream,
        IStructDocNode rootData,
        ISerializerProvider serializerProvider,
        ILoggerFactory? loggerFactory = null)
    {
        _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;
        _logger = _loggerFactory.CreateLogger<StructDocSerializerExecutor>();
        _loggerForTask = _loggerFactory.CreateLogger<SerializerTask>();
        _stream = stream;
        _serializerProvider = serializerProvider;
        LogChildTaskCreated(_logger, rootData.GetType());
        _pendingTasks = new([new SerializerTask(rootData,
                                                logger: _loggerForTask)]);
    }

    /// <summary>Processes all pending tasks in the queue until the entire document tree has been serialized.</summary>
    public void ExecuteTasks()
    {
        LogTasksExecutionStarted(_logger);
        while (HasPendingTasks)
        {
            var firstListNode = _pendingTasks.First
                              ?? throw new InvalidOperationException("No task to process");

            _pendingTasks.Remove(firstListNode);

            ExecuteTask(firstListNode.Value);
        }
        LogTasksExecutionFinished(_logger);
    }

    private void ExecuteTask(SerializerTask task)
    {
        task.Serializer = _serializerProvider.GetSerializerFor(task.Data);
        if (task.Serializer is null)
        {
            task.OnToProcess(_stream);
            task.HasChildTasks = false;
            task.OnProcessed(_stream);
            throw new InvalidOperationException($"No serializer found for {task.Data.GetType()}");
        }

        LogSerializerUtilization(_logger, task.Serializer.GetType());

        task.OnToProcess(_stream);

        NodeSerializationResult serializationResult = task.Serializer.Serialize(task.Data, _stream);

        bool hasChildTasks = false;
        if (!serializationResult.ContentSerialized
            && task.Data is IStructDocNodeHolder childHolderData)
        {
            SerializerTask? nextTask = null;
            foreach (var child in childHolderData.Content.Reverse())
            {
                hasChildTasks = true;
                LogChildTaskCreated(_logger, child.GetType());
                var childTask = new SerializerTask(child,
                                                   new SerializerTaskParentInfo(task),
                                                   _loggerForTask);
                _pendingTasks.AddFirst(childTask);

                if (nextTask is not null)
                {
                    nextTask.ParentInfo!.PreviousSibling = childTask;
                    childTask.ParentInfo!.NextSibling = nextTask;
                }
                nextTask = childTask;
            }
        }
        task.IgnoreChildren = serializationResult.ContentSerialized;
        task.HasChildTasks = hasChildTasks;

        task.OnProcessed(_stream);
    }

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Task execution started")]
    private static partial void LogTasksExecutionStarted(ILogger<StructDocSerializerExecutor> logger);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Task execution finished")]
    private static partial void LogTasksExecutionFinished(ILogger<StructDocSerializerExecutor> logger);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Serializer {serializerType} will be used.")]
    private static partial void LogSerializerUtilization(ILogger<StructDocSerializerExecutor> logger, Type serializerType);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Child task created for {childType}.")]
    private static partial void LogChildTaskCreated(ILogger<StructDocSerializerExecutor> logger, Type childType);
}
