using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public interface IStructDocSerializer
{
    void Serialize(TextWriter stream, IStructDocNode rootData);
}
