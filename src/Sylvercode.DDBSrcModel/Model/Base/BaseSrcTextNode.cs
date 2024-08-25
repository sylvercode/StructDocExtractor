using Sylvercode.DDBSrcModel.Html;

namespace Sylvercode.DDBSrcModel.Model.Base;

public abstract class BaseSrcTextNode<P>(string text, string id) :
    BaseSrcNode<P>(id)
    where P : class, ISrcNodeHolder
{
    public string Text { get; protected set; } = text;
}

public abstract class BaseSrcTextNode(string text, string id)
    : BaseSrcTextNode<ISrcNodeHolder>(text, id)
{
}
