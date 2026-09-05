using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model.Base;

/// <summary>Abstract base for HTML hyperlink-like blocks that carry a URI and text content.</summary>
/// <typeparam name="TParent">The concrete parent holder type that can contain this node.</typeparam>
public abstract class BaseHtmlHref<TParent>(string text, string href, string id = "") :
    BaseStructDocBlockWithAnyParentAndContent(id),
    IStructDocReferencer
    where TParent : class, IStructDocNodeHolder
{
    public string Text { get; protected set; } = text;

    public string Href { get; private set; } = href;

    public bool ContentIsTextOnly => Content.Count == 1 && Content.Single() is PlainTextNode;

    public string TextContent => ContentIsTextOnly ? ((PlainTextNode)Content.Single()).Text : string.Empty;

    #region IStructDocReferencer
    public string GetReference() => Href;

    public IStructDocReferencer.ReferenceType GetReferenceType()
        => IStructDocReferencer.ReferenceType.External;

    public void UpdateReference(string newReference) => Href = newReference;
    #endregion
}
