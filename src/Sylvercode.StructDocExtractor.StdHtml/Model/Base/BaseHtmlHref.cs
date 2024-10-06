using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.StdHtml.Model.Base;

public class BaseHtmlHref<TParent>(string href, string text, string id) :
    BaseHtmlTextNode<TParent>(text, id),
    IStructDocReferencer
    where TParent : class, IStructDocNodeHolder
{
    public string Href { get; private set; } = href;

    public string GetReference() => Href;

    public void UpdateReference(string newReference) => Href = newReference;
}

public class BaseHtmlHref(string href, string text, string id)
    : BaseHtmlHref<IStructDocNodeHolder>(href, text, id)
{
}
