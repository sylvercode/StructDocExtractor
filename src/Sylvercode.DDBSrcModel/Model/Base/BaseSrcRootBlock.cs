namespace Sylvercode.DDBSrcModel.Model.Base;

public abstract class BaseSrcRootBlock<C>(string id) : BaseSrcBlockWithAnyParent<C>(id)
    where C : class, ISrcNode
{
    override public bool IsRoot => true;

    override public ISrcNodeHolder Parent
    {
        get => this;
    }

    override protected void SetParent(ISrcNodeHolder parent)
    {
        throw new InvalidOperationException($"Root node cannot set parent");
    }
}
