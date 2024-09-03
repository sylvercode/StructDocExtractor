namespace Sylvercode.StructDocExtractor.Model.Base;

public abstract class BaseStructDocBlockWithAnyParent<TChild>(string id) :
    BaseStructDocBlock<IStructDocNodeHolder, TChild>(id)
    where TChild : class, IStructDocNode
{

}
