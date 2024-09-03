using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.StdHtml.Model.Base;

public class BaseHtmlHref<TParent>(string href, string text, string id) :
    BaseHtmlTextNode<TParent>(text, id)
    where TParent : class, IStructDocNodeHolder
{
    public string Href { get; } = href;
}

public class BaseHtmlHref(string href, string text, string id)
    : BaseHtmlHref<IStructDocNodeHolder>(href, text, id)
{
}
