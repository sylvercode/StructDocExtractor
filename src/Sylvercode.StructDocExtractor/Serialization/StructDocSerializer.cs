namespace Sylvercode.StructDocExtractor.Serialization;

public class StructDocSerializer(ISerializerProvider serializerProvider)
{
    public void Serialize(StreamWriter stream, object rootData)
    {
        var executor = new StructDocSerializerExecutor(stream, rootData, serializerProvider);
        executor.ExecuteTasks();
    }
}
