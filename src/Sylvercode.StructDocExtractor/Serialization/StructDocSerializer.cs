using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public class StructDocSerializer(ISerializerProvider serializerProvider, ITextWriterAdapterProvider? textWriterAdapterProvider = null, ILoggerFactory? loggerFactory = null)
    : IStructDocSerializer
{
    public ITextWriterAdapterProvider? TextWriterAdapterProvider { get; } = textWriterAdapterProvider;
    public void Serialize(TextWriter stream, IStructDocNode rootData)
    {
        var executor = new StructDocSerializerExecutor(stream, rootData, serializerProvider, loggerFactory);
        executor.ExecuteTasks();
    }
}
