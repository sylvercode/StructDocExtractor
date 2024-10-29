namespace Sylvercode.StructDocExtractor.Model;

public interface IStructDocNodeHolder : IStructDocNode
{
    IReadOnlyList<IStructDocNode> Content { get; }
    bool HasContent { get; }
}

public interface IStructDocNodeHolder<out TChild> : IStructDocNodeHolder
    where TChild : class, IStructDocNode
{
    new IReadOnlyList<TChild> Content { get; }

    IReadOnlyList<IStructDocNode> IStructDocNodeHolder.Content => Content;
}
