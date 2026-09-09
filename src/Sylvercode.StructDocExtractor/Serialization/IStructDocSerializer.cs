using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

/// <summary>Top-level contract for serializing an entire structured document tree to a <see cref="TextWriter"/>.</summary>
public interface IStructDocSerializer
{
    /// <summary>Gets the optional provider used to create the <see cref="TextWriter"/> wrapping an output stream.</summary>
    ITextWriterProvider? TextWriterProvider { get; }

    /// <summary>Serializes the document tree rooted at <paramref name="rootData"/> to <paramref name="stream"/>.</summary>
    /// <param name="stream">The writer to which the serialized document is written.</param>
    /// <param name="rootData">The root node of the structured document tree to serialize.</param>
    void Serialize(TextWriter stream, IStructDocNode rootData);
}
