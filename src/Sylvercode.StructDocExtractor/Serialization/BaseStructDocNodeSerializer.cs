using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public abstract partial class BaseStructDocNodeSerializer<TData, TWriter>(ILogger? logger = null) : IStructDocNodeSerializer<TData, TWriter>, ISerializer<TData, TWriter>
    where TData : IStructDocNode
    where TWriter : TextWriter
{
    private readonly ILogger _logger = logger ?? NullLogger.Instance;

    public NodeSerializationResult Serialize(TData obj, TWriter stream)
    {
        LogSerialize(obj.GetType());
        NodeSerializationResult result = new();
        Serialize(obj, stream, result);
        return result;
    }

    void IStructDocNodeSerializer<TData, TWriter>.OnBeforeChildSerialize(TData node, TData? previousNode, TWriter stream)
    {
        LogOnBeforeChildSerialize(node.GetType(), previousNode?.GetType());
        OnBeforeChildSerialize(node, previousNode, stream);
    }

    void IStructDocNodeSerializer<TData, TWriter>.OnAfterChildSerialize(TData node, TData? nextNode, TWriter stream)
    {
        LogOnAfterChildSerialize(node.GetType(), nextNode?.GetType());
        OnAfterChildSerialize(node, nextNode, stream);
    }

    protected virtual void Serialize(TData obj, TWriter stream, NodeSerializationResult result)
    {

    }

    protected virtual void OnBeforeChildSerialize(TData node, TData? previousNode, TWriter stream)
    {

    }

    protected virtual void OnAfterChildSerialize(TData node, TData? nextNode, TWriter stream)
    {

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

public abstract partial class BaseStructDocNodeSerializer<TData>(ILogger? logger = null) : BaseStructDocNodeSerializer<TData, TextWriter>(logger)
    where TData : IStructDocNode
{

}
