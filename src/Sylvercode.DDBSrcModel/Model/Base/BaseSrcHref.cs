namespace Sylvercode.DDBSrcModel.Model.Base;

public class BaseSrcHref<P>(string href, string text, string id) :
    BaseSrcTextNode<P>(text, id)
    where P : class, ISrcNodeHolder
{
    public string Href { get; } = href;
}

public class BaseSrcHref(string href, string text, string id)
    : BaseSrcHref<ISrcNodeHolder>(href, text, id)
{
}
