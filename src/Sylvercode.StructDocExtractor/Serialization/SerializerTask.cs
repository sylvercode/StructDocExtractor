using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

/// <summary>Carries serialization state for a single <see cref="IStructDocNode"/> and manages its lifecycle callbacks within the serialization pipeline.</summary>
/// <remarks>
/// Each task is associated with a node (<see cref="Data"/>), its resolved <see cref="Serializer"/>, and an
/// optional <see cref="ParentInfo"/> linking it to adjacent siblings and its parent task.
/// <see cref="OnToProcess"/> and <see cref="OnProcessed"/> coordinate the before/between/after notification
/// protocol between <see cref="IStructDocNodeSerializer"/> and <see cref="IStructDocNodeHolderSerializer"/>
/// implementations so that spacing, opening, and closing tokens are emitted at the correct points.
/// </remarks>
public partial class SerializerTask(IStructDocNode data, SerializerTaskParentInfo? parentInfo = null, ILogger<SerializerTask>? logger = null)
{
    /// <summary>Gets the structural document node this task will serialize.</summary>
    public IStructDocNode Data { get; } = data;

    /// <summary>Gets the parent-context linking this task to its parent task and adjacent siblings, or <see langword="null"/> for the root task.</summary>
    public SerializerTaskParentInfo? ParentInfo { get; } = parentInfo;

    /// <summary>Gets or sets a value indicating whether child tasks should be skipped because the serializer wrote child content directly.</summary>
    public bool IgnoreChildren { get; set; }

    /// <summary>Gets or sets a value indicating whether child tasks were enqueued for this node; <see langword="null"/> until <see cref="OnProcessed"/> is called.</summary>
    public bool? HasChildTasks { get; set; }

    /// <summary>Gets or sets the serializer resolved for <see cref="Data"/>'s runtime type.</summary>
    public ISerializer? Serializer { get; set; }
    private readonly ILogger<SerializerTask> _logger = logger ?? NullLogger<SerializerTask>.Instance;

    /// <summary>Fires the between-children notification on the parent holder and the before-child notification on this node's serializer.</summary>
    /// <param name="stream">The writer receiving serialized output.</param>
    public void OnToProcess(TextWriter stream)
    {
        LogTaskProcessStarted(Data.GetType(), Serializer?.GetType());
        IStructDocNode? node = Data as IStructDocNode;
        SerializerTask? parentTask = ParentInfo?.Parent;
        if (parentTask is not null)
        {
            LogTaskProcessWitParentStarted(parentTask.Data.GetType(), parentTask.Serializer?.GetType());
            IStructDocNodeHolder? holder = parentTask.Data as IStructDocNodeHolder;
            IStructDocNodeHolderSerializer? holderSerializer = parentTask.Serializer as IStructDocNodeHolderSerializer;
            if (CanNotifyHolder(holder, holderSerializer))
            {
                LogNotifyHolder();
                holderSerializer.OnBetweenChildrenSerialize(
                    holder,
                    ParentInfo!.PreviousSibling?.Data as IStructDocNode,
                    node,
                    stream);
            }
        }

        IStructDocNodeSerializer? nodeSerializer = Serializer as IStructDocNodeSerializer;
        if (CanNotifyNode(node, nodeSerializer, ParentInfo))
        {
            LogNotifyNode();
            nodeSerializer.OnBeforeAsChildSerialize(node, ParentInfo.PreviousSibling?.Data as IStructDocNode, stream);
        }
    }

    /// <summary>Called after this task's node has been serialized; fires the after-child notification or defers it until all children complete.</summary>
    /// <param name="stream">The writer receiving serialized output.</param>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="HasChildTasks"/> was not set before calling this method.</exception>
    public void OnProcessed(TextWriter stream)
    {
        if (HasChildTasks is null)
            throw new InvalidOperationException("HasChildTasks is not set");

        if (!HasChildTasks.Value)
            OnProcessLastChild(stream, null);
        else
            LogWaitingForChildrenToEnd();
    }

    private void OnProcessLastChild(TextWriter stream, SerializerTask? lastChild)
    {
        LogLastChildProcessed();
        IStructDocNodeHolder? nodeHolder = Data as IStructDocNodeHolder;
        IStructDocNodeHolderSerializer? nodeHolderSerializer = Serializer as IStructDocNodeHolderSerializer;
        if (!IgnoreChildren
            && CanNotifyHolder(nodeHolder, nodeHolderSerializer))
        {
            LogNotifyHolder();
            nodeHolderSerializer.OnBetweenChildrenSerialize(nodeHolder, lastChild?.Data as IStructDocNode, null, stream);
        }

        IStructDocNode? node = Data as IStructDocNode;
        IStructDocNodeSerializer? nodeSerializer = Serializer as IStructDocNodeSerializer;
        if (CanNotifyNode(node, nodeSerializer, ParentInfo))
        {
            LogNotifyNode();
            nodeSerializer.OnAfterAsChildSerialize(node, ParentInfo.NextSibling?.Data as IStructDocNode, stream);
        }

        if (ParentInfo is not null
            && ParentInfo.IsLastChild)
            ParentInfo.Parent.OnProcessLastChild(stream, this);
    }


    /// <summary>Returns <see langword="true"/> when both <paramref name="node"/> and <paramref name="nodeSerializer"/> are non-null and the task has a <paramref name="parentInfo"/>.</summary>
    public static bool CanNotifyNode(
        [NotNullWhen(true)] IStructDocNode? node,
        [NotNullWhen(true)] IStructDocNodeSerializer? nodeSerializer,
        [NotNullWhen(true)] SerializerTaskParentInfo? parentInfo)
        => nodeSerializer is not null && node is not null && parentInfo is not null;

    /// <summary>Returns <see langword="true"/> when both <paramref name="holder"/> and <paramref name="holderSerializer"/> are non-null.</summary>
    public static bool CanNotifyHolder(
        [NotNullWhen(true)] IStructDocNodeHolder? holder,
        [NotNullWhen(true)] IStructDocNodeHolderSerializer? holderSerializer)
        => holderSerializer is not null && holder is not null;

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Task for {dataType} started with serializer {serializerType}")]
    private partial void LogTaskProcessStarted(Type dataType, Type? serializerType);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Task for {dataType} started with parent serializer {serializerType}")]
    private partial void LogTaskProcessWitParentStarted(Type dataType, Type? serializerType);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Notifying holder")]
    private partial void LogNotifyHolder();

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Notifying node")]
    private partial void LogNotifyNode();

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Waiting for children to end")]
    private partial void LogWaitingForChildrenToEnd();

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Last child (or no child) processed")]
    private partial void LogLastChildProcessed();
}
