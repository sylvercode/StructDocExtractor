namespace Sylvercode.StructDocExtractor.Serialization;

public interface ISerializerProvider
{
    ISerializer GetSerializerFor(object obj);
}
