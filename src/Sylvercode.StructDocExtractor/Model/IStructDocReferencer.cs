namespace Sylvercode.StructDocExtractor.Model;

/// <summary>Contract for structural nodes that carry a URI reference, such as hyperlinks or embedded images.</summary>
public interface IStructDocReferencer
{
    /// <summary>Specifies how a URI reference is incorporated into the document.</summary>
    public enum ReferenceType
    {
        /// <summary>The reference target is embedded within the document (e.g., an inline image).</summary>
        Embeded,

        /// <summary>The reference points to an external resource opened via navigation.</summary>
        External
    }

    /// <summary>Returns the <see cref="ReferenceType"/> indicating how this reference is used.</summary>
    /// <returns>The reference type for this node.</returns>
    ReferenceType GetReferenceType();

    /// <summary>Returns the URI reference string carried by this node.</summary>
    /// <returns>The reference URI as a string.</returns>
    string GetReference();

    /// <summary>Replaces the current URI reference with a new value.</summary>
    /// <param name="newReference">The new URI reference string to assign.</param>
    void UpdateReference(string newReference);
}
