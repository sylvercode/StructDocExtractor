namespace Sylvercode.DDBSrcModel.Model;

public interface ISrcNodeHolder : ISrcNode
{
    IReadOnlyList<ISrcNode> Content { get; }
}

public interface ISrcNodeHolder<out C> : ISrcNodeHolder
    where C : class, ISrcNode
{
    new IReadOnlyList<C> Content { get; }

    IReadOnlyList<ISrcNode> ISrcNodeHolder.Content => Content;
}
