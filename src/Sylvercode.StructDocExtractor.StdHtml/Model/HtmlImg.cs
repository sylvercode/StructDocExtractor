using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.StructDocExtractor.StdHtml.Model;

/// <summary>Node representing an HTML <c>&lt;img&gt;</c> element, carrying a source URI and implementing <see cref="IStructDocReferencer"/>.</summary>
/// <param name="src">The URI of the image source.</param>
/// <param name="id">The optional node identifier.</param>
public class HtmlImg(string src, string id = "") :
    BaseStructDocNode(id),
    IStructDocReferencer
{
    /// <summary>Gets the URI of the image source.</summary>
    public string Src { get; private set; } = src;

    #region IStructDocReferencer
    /// <inheritdoc/>
    public IStructDocReferencer.ReferenceType GetReferenceType()
        => IStructDocReferencer.ReferenceType.Embeded;

    /// <inheritdoc/>
    public string GetReference() => Src;

    /// <inheritdoc/>
    public void UpdateReference(string newReference) => Src = newReference;
    #endregion
}
