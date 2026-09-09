namespace Sylvercode.StructDocExtractor.Model;

/// <summary>Marker contract identifying block-level structural document nodes that are simultaneously a node and a holder of child nodes.</summary>
public interface IStructDocBlock : IStructDocNode, IStructDocNodeHolder
{

}

/// <summary>Strongly-typed block contract that constrains both the parent holder type and the child node type.</summary>
/// <typeparam name="TParent">The type of node holder that can contain this block.</typeparam>
/// <typeparam name="TChild">The type of child nodes this block can contain.</typeparam>
public interface IStructDocBlock<out TParent, out TChild> : IStructDocBlock, IStructDocNode<TParent>, IStructDocNodeHolder<TChild>
    where TParent : class, IStructDocNodeHolder
    where TChild : class, IStructDocNode
{

}
