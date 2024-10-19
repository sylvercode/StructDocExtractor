namespace Sylvercode.StructDocExtractor.Serialization;

public interface ITextWriterProvider
{
    TextWriter GetTextWriter(Stream stream);
}
