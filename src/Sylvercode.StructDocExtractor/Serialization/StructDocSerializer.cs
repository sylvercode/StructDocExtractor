using Microsoft.Extensions.Logging;

namespace Sylvercode.StructDocExtractor.Serialization;

public class StructDocSerializer(ISerializerProvider serializerProvider, ILoggerFactory loggerFactory)
{
    public void Serialize(StreamWriter stream, object rootData)
    {
        var executor = new StructDocSerializerExecutor(stream, rootData, serializerProvider, loggerFactory);
        executor.ExecuteTasks();
    }
}
