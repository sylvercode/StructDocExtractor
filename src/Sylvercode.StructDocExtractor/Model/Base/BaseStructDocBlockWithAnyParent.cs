namespace Sylvercode.StructDocExtractor.Model.Base;

/// <summary>Abstract base for block nodes that can be contained by any <see cref="IStructDocNodeHolder"/>, imposing no restriction on parent type.</summary>
/// <typeparam name="TChild">The type of child nodes this block can contain.</typeparam>
public abstract class BaseStructDocBlockWithAnyParent<TChild>(string id) :
    BaseStructDocBlock<IStructDocNodeHolder, TChild>(id)
    where TChild : class, IStructDocNode
{

}
