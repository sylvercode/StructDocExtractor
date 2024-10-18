using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model.Base;

public class BaseHtmlHref<TParent>(string href, string id) :
    BaseStructDocNode<TParent>(id),
    IStructDocReferencer
    where TParent : class, IStructDocNodeHolder
{
    public string Href { get; private set; } = href;

    public string GetReference() => Href;

    public void UpdateReference(string newReference) => Href = newReference;
}

public class BaseHtmlHref(string href, string id)
    : BaseHtmlHref<IStructDocNodeHolder>(href, id)
{
}
