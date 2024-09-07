namespace Sylvercode.StructDocExtractor.Serialization;

public interface ISerializer
{
    void Serialize(object obj, StreamWriter stream);
}

public interface ISerializer<in TInput> : ISerializer
{
    void Serialize(TInput obj, StreamWriter stream);
    void ISerializer.Serialize(object obj, StreamWriter stream)
        => Serialize((TInput)obj, stream);
}
