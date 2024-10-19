namespace Sylvercode.StructDocExtractor.Serialization;

public interface ITextWriterAdapterProvider
{
    TextWriter GetTextWriterAdapter(Stream stream);
}
