using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

/// <summary>Default <see cref="IStructDocSerializer"/> that drives the task-based serialization pipeline.</summary>
/// <remarks>
/// Creates a new <see cref="StructDocSerializerExecutor"/> per <see cref="Serialize"/> call and delegates
/// node dispatch to the injected <see cref="ISerializerProvider"/>.
/// </remarks>
public class StructDocSerializer(ISerializerProvider serializerProvider, ITextWriterProvider? textWriterAdapterProvider = null, ILoggerFactory? loggerFactory = null)
    : IStructDocSerializer
{
    /// <inheritdoc/>
    public ITextWriterProvider? TextWriterProvider { get; } = textWriterAdapterProvider;

    /// <inheritdoc/>
    public void Serialize(TextWriter stream, IStructDocNode rootData)
    {
        var executor = new StructDocSerializerExecutor(stream, rootData, serializerProvider, loggerFactory);
        executor.ExecuteTasks();
    }
}
