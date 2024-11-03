namespace Sylvercode.StructDocExtractor.Model;

public interface IStructDocNode
{
    IStructDocNodeHolder Parent { get; }
    string Id { get; init; }

    bool IsRoot { get; }
    bool IsLeaf { get; }

    string DebugName => $"{GetType().Name}({Id})";

    string NodeSnippet();
}

public interface IStructDocNode<out TParent> : IStructDocNode
    where TParent : class, IStructDocNodeHolder
{
    new TParent Parent { get; }
    IStructDocNodeHolder IStructDocNode.Parent { get => Parent; }
}
