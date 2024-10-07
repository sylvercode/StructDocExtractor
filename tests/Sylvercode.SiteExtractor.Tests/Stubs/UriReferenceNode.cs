using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Model.Base;

namespace Sylvercode.SiteExtractor.Tests.Stubs;

public class UriReferenceNode(string uri, string reference) : BaseStructDocNode(uri), IStructDocReferencer
{
    public string Reference { get; set; } = reference;

    public string GetReference()
        => Reference;

    public void UpdateReference(string newReference)
        => Reference = newReference;
}
