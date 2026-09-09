namespace Sylvercode.StructDocExtractor.Model.Base;

/// <summary>Abstract base for block nodes that impose no restrictions on either parent type or child content type.</summary>
public abstract class BaseStructDocBlockWithAnyParentAndContent(string id) :
    BaseStructDocBlock<IStructDocNodeHolder, IStructDocNode>(id)
{

}
