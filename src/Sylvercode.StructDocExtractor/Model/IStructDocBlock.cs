namespace Sylvercode.StructDocExtractor.Model;

public interface IStructDocBlock : IStructDocNode, IStructDocNodeHolder
{

}

public interface IStructDocBlock<out TParent, out TChild> : IStructDocBlock, IStructDocNode<TParent>, IStructDocNodeHolder<TChild>
    where TParent : class, IStructDocNodeHolder
    where TChild : class, IStructDocNode
{

}
