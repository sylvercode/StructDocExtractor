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
    /// <summary>Gets the text content associated with this hyperlink node.</summary>
    public string Text { get; protected set; } = text;

    /// <summary>Gets the URI of the hyperlink.</summary>
    public string Href { get; private set; } = href;

    /// <summary>Gets a value indicating whether the node's content consists of a single plain-text child.</summary>
    public bool ContentIsTextOnly => Content.Count == 1 && Content.Single() is PlainTextNode;

    /// <summary>Gets the plain text content when <see cref="ContentIsTextOnly"/> is <see langword="true"/>; otherwise an empty string.</summary>
    public string TextContent => ContentIsTextOnly ? ((PlainTextNode)Content.Single()).Text : string.Empty;

    #region IStructDocReferencer
    /// <inheritdoc/>
    public string GetReference() => Href;

    /// <inheritdoc/>
    public IStructDocReferencer.ReferenceType GetReferenceType()
        => IStructDocReferencer.ReferenceType.External;

    /// <inheritdoc/>
    public void UpdateReference(string newReference) => Href = newReference;
    #endregion
}
