namespace Sylvercode.StructDocExtractor.Model;

public interface IStructDocReferencer
{
    string GetReference();
    void UpdateReference(string newReference);
}
