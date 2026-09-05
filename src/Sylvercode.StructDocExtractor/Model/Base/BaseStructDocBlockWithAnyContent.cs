namespace Sylvercode.StructDocExtractor.Model.Base;

/// <summary>Abstract base for block nodes that accept any <see cref="IStructDocNode"/> as children, imposing no restriction on child type.</summary>
/// <typeparam name="TParent">The type of the parent node holder.</typeparam>
public abstract class BaseStructDocBlockWithAnyContent<TParent>(string id) :
    BaseStructDocBlock<TParent, IStructDocNode>(id)
    where TParent : class, IStructDocNodeHolder
{

}
