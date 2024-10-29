using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

public class HtmlAnchor(string href, string id) :
    BaseStructDocBlockWithAnyParentAndContent(id),
    IStructDocReferencer
{
    public HtmlAnchor(string href) : this(href, string.Empty)
    {
    }

    public string Href { get; private set; } = href;

    public bool ContentIsTextOnly => Content.Count == 1 && Content.Single() is PlainTextNode;

    public string TextContent => ContentIsTextOnly ? ((PlainTextNode)Content.Single()).Text : string.Empty;

    public string GetReference() => Href;

    public void UpdateReference(string newReference) => Href = newReference;
}
