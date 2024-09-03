namespace Sylvercode.StructDocExtractor.Model.Base;

public abstract class BaseStructDocRootBlock<TChild>(string id) : BaseStructDocBlockWithAnyParent<TChild>(id)
    where TChild : class, IStructDocNode
{
    override public bool IsRoot => true;

    override public IStructDocNodeHolder Parent
    {
        get => this;
    }

    override protected void SetParent(IStructDocNodeHolder parent)
    {
        throw new InvalidOperationException($"Root node cannot set parent");
    }
}
