namespace Sylvercode.DDBSrcModel.Model.Base;

public abstract class BaseSrcBlockWithAnyContent<P>(string id) :
    BaseSrcBlock<P, ISrcNode>(id)
    where P : class, ISrcNodeHolder
{

}
