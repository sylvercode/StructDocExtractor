using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public partial class BaseStructDocNodeSerializer<TData, TWriter>(
    BaseStructDocNodeSerializer<TData, TWriter>.Handler? handler = null,
    ILogger? logger = null)
    : IStructDocNodeSerializer<TData, TWriter>, ISerializer<TData, TWriter>
    where TData : IStructDocNode
    where TWriter : TextWriter
{

    public class Handler : IStructDocNodeSerializer<TData, TWriter>
    {
        public virtual void OnAfterChildSerialize(TData node, IStructDocNode? nextNode, TWriter stream)
        {
        }

        public virtual void Serialize(TData obj, TWriter stream, NodeSerializationResult result)
        {

        }

        public virtual void OnBeforeChildSerialize(TData node, IStructDocNode? previousNode, TWriter stream)
        {
        }
    }

    private readonly ILogger _logger = logger ?? NullLogger.Instance;

    public NodeSerializationResult Serialize(TData obj, TWriter stream)
    {
        LogSerialize(obj.GetType());
        NodeSerializationResult result = new();
        handler?.Serialize(obj, stream, result);
        return result;
    }

    public void OnBeforeChildSerialize(TData node, IStructDocNode? previousNode, TWriter stream)
    {
        LogOnBeforeChildSerialize(node.GetType(), previousNode?.GetType());
        handler?.OnBeforeChildSerialize(node, previousNode, stream);
    }

    public void OnAfterChildSerialize(TData node, IStructDocNode? nextNode, TWriter stream)
    {
        LogOnAfterChildSerialize(node.GetType(), nextNode?.GetType());
        handler?.OnAfterChildSerialize(node, nextNode, stream);
    }

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Serialize data for data type {dataType}")]
    private partial void LogSerialize(Type dataType);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Before child serialize for data type {dataType} with previous data type {previousDataType}")]
    private partial void LogOnBeforeChildSerialize(Type dataType, Type? previousDataType);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "After child serialize for data type {dataType} with next data type {nextDataType}")]
    private partial void LogOnAfterChildSerialize(Type dataType, Type? nextDataType);
}

public abstract partial class BaseStructDocNodeSerializer<TData>(
    BaseStructDocNodeSerializer<TData, TextWriter>.Handler? handler = null,
     ILogger? logger = null)
    : BaseStructDocNodeSerializer<TData, TextWriter>(handler, logger)
    where TData : IStructDocNode
{

}
