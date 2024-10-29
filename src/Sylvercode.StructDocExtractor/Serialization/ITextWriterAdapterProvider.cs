namespace Sylvercode.StructDocExtractor.Serialization;

public interface ITextWriterProvider
{
    TextWriter GetTextWriter(Stream stream);
}

public interface ITextWriterProvider<TWriter> : ITextWriterProvider
    where TWriter : TextWriter
{
    new TWriter GetTextWriter(Stream stream);
    TextWriter ITextWriterProvider.GetTextWriter(Stream stream) => GetTextWriter(stream);
}
