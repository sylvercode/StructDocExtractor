using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.SiteExtractor.StdHtml.Tests.Stubs;

/// <summary>Stub <see cref="IStructDocReferencer"/> for verifying HTML dependency filter behaviour.</summary>
public class StubReferencer(
    string reference,
    IStructDocReferencer.ReferenceType referenceType = IStructDocReferencer.ReferenceType.External
) : IStructDocReferencer
{
    /// <inheritdoc/>
    public string GetReference() => reference;

    /// <inheritdoc/>
    public IStructDocReferencer.ReferenceType GetReferenceType()
        => referenceType;

    /// <inheritdoc/>
    public void UpdateReference(string newReference)
        => throw new NotImplementedException();
}
