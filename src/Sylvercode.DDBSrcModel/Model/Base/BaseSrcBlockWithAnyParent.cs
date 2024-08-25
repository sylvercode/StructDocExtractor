namespace Sylvercode.DDBSrcModel.Model.Base;

public abstract class BaseSrcBlockWithAnyParent<C>(string id) :
    BaseSrcBlock<ISrcNodeHolder, C>(id)
    where C : class, ISrcNode
{

}
