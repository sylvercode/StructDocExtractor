namespace Sylvercode.DDBSrcModel.Model;

public interface ISrcNode
{
    ISrcNodeHolder Parent { get; }
    string Id { get; }

    bool IsRoot { get; }
    bool IsLeaf { get; }

    string DebugName => $"{GetType().Name}({Id})";

    string NodeSnippet();
}

public interface ISrcNode<out P> : ISrcNode
    where P : class, ISrcNodeHolder
{
    new P Parent { get; }
    ISrcNodeHolder ISrcNode.Parent { get => Parent; }
}
