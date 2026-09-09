namespace Sylvercode.StructDocExtractor.Model.Base;

/// <summary>Abstract base for root blocks that accept any <see cref="IStructDocNode"/> as children.</summary>
public abstract class BaseStructDocRootBlockWithAnyContent(string id) : BaseStructDocRootBlock<IStructDocNode>(id)
{
}
