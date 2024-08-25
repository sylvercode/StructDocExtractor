namespace Sylvercode.DDBSrcModel.Model.Base;

public abstract class BaseSrcBlockWithAnyParentAndContent(string id) :
    BaseSrcBlock<ISrcNodeHolder, ISrcNode>(id)
{

}
