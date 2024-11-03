namespace Sylvercode.StructDocExtractor.Model;

public interface IStructDocReferencer
{
    public enum ReferenceType
    {
        Embeded,
        External
    }

    ReferenceType GetReferenceType();
    string GetReference();
    void UpdateReference(string newReference);
}
