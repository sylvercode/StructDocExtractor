using System.Diagnostics.CodeAnalysis;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public class StructDocSerializerExecutor(StreamWriter stream, object rootData, ISerializerProvider serializerProvider)
{

    private readonly LinkedList<SerializerTask> _pendingTasks = new([new SerializerTask(rootData)]);

    private bool HasPendingTasks => _pendingTasks.Count > 0;

    public void ExecuteTasks()
    {
        while (HasPendingTasks)
        {
            var firstListNode = _pendingTasks.First
                              ?? throw new InvalidOperationException("No task to process");

            _pendingTasks.Remove(firstListNode);

            ExecuteTask(firstListNode.Value);
        }
    }

    private void ExecuteTask(SerializerTask task)
    {
        IStructDocNode? node = task.Data as IStructDocNode;
        IStructDocNodeHolder? holder = task.ParentInfo?.Parent.Data as IStructDocNodeHolder;
        IStructDocNodeHolderSerializer? holderSerializer = task.ParentInfo?.Parent.Serializer as IStructDocNodeHolderSerializer;

        if (CanNotifyHolder(holder, holderSerializer, node))
            holderSerializer.OnBetweenChildrenSerialize(holder, (IStructDocNode?)task.ParentInfo?.PreviousSibling?.Data, node, stream);

        task.Serializer = serializerProvider.GetSerializerFor(task.Data);
        if (task.Serializer is null)
            throw new InvalidOperationException($"No serializer found for {task.Data.GetType()}");

        IStructDocNodeSerializer? nodeSerializer = task.Serializer as IStructDocNodeSerializer;

        if (CanNotifyNode(node, nodeSerializer))
            nodeSerializer.OnBeforeChildSerialize(node, task.ParentInfo?.PreviousSibling?.Data as IStructDocNode, stream);

        task.Serializer.Serialize(task.Data, stream);

        if (CanNotifyNode(node, nodeSerializer))
            nodeSerializer.OnAfterChildSerialize(node, task.ParentInfo?.NextSibling?.Data as IStructDocNode, stream);

        if (CanNotifyHolder(holder, holderSerializer, node)
            && (task.ParentInfo?.IsLastChild ?? false))
            holderSerializer.OnBetweenChildrenSerialize(holder, node, null, stream);

        if (task.Data is not IStructDocNodeHolder childHolderData)
            return;

        if (childHolderData.Content.Count == 0)
        {
            if (nodeSerializer is IStructDocNodeHolderSerializer childHolderSerializer)
                childHolderSerializer.OnBetweenChildrenSerialize(childHolderData, null, null, stream);
            return;
        }

        SerializerTask? previousTask = null;
        foreach (var child in childHolderData.Content)
        {
            var childTask = new SerializerTask(child, new SerializerTaskParentInfo(task));
            _pendingTasks.AddFirst(childTask);

            if (previousTask is not null)
            {
                previousTask.ParentInfo!.NextSibling = childTask;
                childTask.ParentInfo!.PreviousSibling = previousTask;
            }
            previousTask = childTask;
        }
    }


    private static bool CanNotifyHolder(
        [NotNullWhen(true)] IStructDocNodeHolder? holder,
        [NotNullWhen(true)] IStructDocNodeHolderSerializer? holderSerializer,
        [NotNullWhen(true)] IStructDocNode? node)
        => holderSerializer is not null && holder is not null && node is not null;

    private static bool CanNotifyNode(
        [NotNullWhen(true)] IStructDocNode? node,
        [NotNullWhen(true)] IStructDocNodeSerializer? nodeSerializer)
        => nodeSerializer is not null && node is not null;
}
