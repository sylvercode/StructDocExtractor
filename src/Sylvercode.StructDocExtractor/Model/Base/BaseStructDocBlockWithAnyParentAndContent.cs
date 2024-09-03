namespace Sylvercode.StructDocExtractor.Model.Base;

public abstract class BaseStructDocBlockWithAnyParentAndContent(string id) :
    BaseStructDocBlock<IStructDocNodeHolder, IStructDocNode>(id)
{

}
