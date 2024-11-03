using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.SiteExtractor.StdHtml.Tests.Stubs;

public class StubReferencer(
    string reference,
    IStructDocReferencer.ReferenceType referenceType = IStructDocReferencer.ReferenceType.External
) : IStructDocReferencer
{
    public string GetReference() => reference;

    public IStructDocReferencer.ReferenceType GetReferenceType()
        => referenceType;

    public void UpdateReference(string newReference)
        => throw new NotImplementedException();
}
