using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public interface IStructDocSerializer
{
    ITextWriterAdapterProvider? TextWriterAdapterProvider { get; }
    void Serialize(TextWriter stream, IStructDocNode rootData);
}
