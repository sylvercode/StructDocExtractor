using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.SiteExtractor.Tests.Stubs;

/// <summary>Stub <see cref="IStructDocReferencer"/> node for verifying URI-reference update behaviour.</summary>
public class UriReferenceNode(string uri, string reference) : BaseStructDocNode(uri), IStructDocReferencer
{
    /// <summary>Gets or sets the reference URI string held by this node.</summary>
    public string Reference { get; set; } = reference;

    /// <inheritdoc/>
    public IStructDocReferencer.ReferenceType GetReferenceType()
        => IStructDocReferencer.ReferenceType.External;

    /// <inheritdoc/>
    public string GetReference()
        => Reference;

    /// <inheritdoc/>
    public void UpdateReference(string newReference)
        => Reference = newReference;
}
