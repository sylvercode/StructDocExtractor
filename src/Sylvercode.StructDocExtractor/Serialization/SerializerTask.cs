using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public partial class SerializerTask(IStructDocNode data, SerializerTaskParentInfo? parentInfo = null, ILogger<SerializerTask>? logger = null)
{
    public IStructDocNode Data { get; } = data;
    public SerializerTaskParentInfo? ParentInfo { get; } = parentInfo;
    public bool? HasChildTasks { get; set; }
    public ISerializer? Serializer { get; set; }
    private readonly ILogger<SerializerTask> _logger = logger ?? NullLogger<SerializerTask>.Instance;

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
            nodeSerializer.OnBeforeChildSerialize(node, ParentInfo.PreviousSibling?.Data as IStructDocNode, stream);
        }
    }

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
        if (CanNotifyHolder(nodeHolder, nodeHolderSerializer))
        {
            LogNotifyHolder();
            nodeHolderSerializer.OnBetweenChildrenSerialize(nodeHolder, lastChild?.Data as IStructDocNode, null, stream);
        }

        IStructDocNode? node = Data as IStructDocNode;
        IStructDocNodeSerializer? nodeSerializer = Serializer as IStructDocNodeSerializer;
        if (CanNotifyNode(node, nodeSerializer, ParentInfo))
        {
            LogNotifyNode();
            nodeSerializer.OnAfterChildSerialize(node, ParentInfo.NextSibling?.Data as IStructDocNode, stream);
        }

        if (ParentInfo is not null
            && ParentInfo.IsLastChild)
            ParentInfo.Parent.OnProcessLastChild(stream, this);
    }


    public static bool CanNotifyNode(
        [NotNullWhen(true)] IStructDocNode? node,
        [NotNullWhen(true)] IStructDocNodeSerializer? nodeSerializer,
        [NotNullWhen(true)] SerializerTaskParentInfo? parentInfo)
        => nodeSerializer is not null && node is not null && parentInfo is not null;

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
