using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public interface IStructDocSerializer
{
    ITextWriterProvider? TextWriterProvider { get; }
    void Serialize(TextWriter stream, IStructDocNode rootData);
}
