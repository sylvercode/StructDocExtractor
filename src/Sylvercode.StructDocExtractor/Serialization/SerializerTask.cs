using System.Diagnostics.CodeAnalysis;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public class SerializerTask(object data, SerializerTaskParentInfo? parentInfo = null)
{
    public object Data { get; } = data;
    public SerializerTaskParentInfo? ParentInfo { get; } = parentInfo;
    public bool? HasChildTasks { get; set; }
    public ISerializer? Serializer { get; set; }

    public void OnToProcess(StreamWriter stream)
    {
        IStructDocNode? node = Data as IStructDocNode;
        SerializerTask? parentTask = ParentInfo?.Parent;
        if (parentTask is not null)
        {
            IStructDocNodeHolder? holder = parentTask.Data as IStructDocNodeHolder;
            IStructDocNodeHolderSerializer? holderSerializer = parentTask.Serializer as IStructDocNodeHolderSerializer;
            if (CanNotifyHolder(holder, holderSerializer))
                holderSerializer.OnBetweenChildrenSerialize(
                    holder,
                    ParentInfo!.PreviousSibling?.Data as IStructDocNode,
                    node,
                    stream);
        }

        IStructDocNodeSerializer? nodeSerializer = Serializer as IStructDocNodeSerializer;
        if (CanNotifyNode(node, nodeSerializer, ParentInfo))
            nodeSerializer.OnBeforeChildSerialize(node, ParentInfo.PreviousSibling?.Data as IStructDocNode, stream);
    }

    public void OnProcessed(StreamWriter stream)
    {
        if (HasChildTasks is null)
            throw new InvalidOperationException("HasChildTasks is not set");

        if (!HasChildTasks.Value)
            OnProcessLastChild(stream, null);
    }

    private void OnProcessLastChild(StreamWriter stream, SerializerTask? lastChild)
    {
        IStructDocNodeHolder? nodeHolder = Data as IStructDocNodeHolder;
        IStructDocNodeHolderSerializer? nodeHolderSerializer = Serializer as IStructDocNodeHolderSerializer;
        if (CanNotifyHolder(nodeHolder, nodeHolderSerializer))
            nodeHolderSerializer.OnBetweenChildrenSerialize(nodeHolder, lastChild?.Data as IStructDocNode, null, stream);

        IStructDocNode? node = Data as IStructDocNode;
        IStructDocNodeSerializer? nodeSerializer = Serializer as IStructDocNodeSerializer;
        if (CanNotifyNode(node, nodeSerializer, ParentInfo))
            nodeSerializer.OnAfterChildSerialize(node, ParentInfo.NextSibling?.Data as IStructDocNode, stream);

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
}
