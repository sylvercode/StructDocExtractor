using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model.Base;

public abstract class BaseHtmlTextNode<TParent>(string text, string id) :
    BaseStructDocNode<TParent>(id)
    where TParent : class, IStructDocNodeHolder
{
    public string Text { get; protected set; } = text;
}

public abstract class BaseHtmlTextNode(string text, string id)
    : BaseHtmlTextNode<IStructDocNodeHolder>(text, id)
{
}
