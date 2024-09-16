using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public class StructDocSerializer(ISerializerProvider serializerProvider, ILoggerFactory loggerFactory)
    : IStructDocSerializer
{
    public void Serialize(StreamWriter stream, IStructDocNode rootData)
    {
        var executor = new StructDocSerializerExecutor(stream, rootData, serializerProvider, loggerFactory);
        executor.ExecuteTasks();
    }
}
