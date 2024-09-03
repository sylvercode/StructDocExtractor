namespace Sylvercode.StructDocExtractor.Model.Base;

public abstract class BaseStructDocBlockWithAnyContent<TParent>(string id) :
    BaseStructDocBlock<TParent, IStructDocNode>(id)
    where TParent : class, IStructDocNodeHolder
{

}
