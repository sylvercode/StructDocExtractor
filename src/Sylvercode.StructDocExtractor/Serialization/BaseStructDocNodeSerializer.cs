using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public partial class BaseStructDocNodeSerializer<TData, TWriter>(ILogger? logger = null) : IStructDocNodeSerializer<TData, TWriter>, ISerializer<TData, TWriter>
    where TData : IStructDocNode
    where TWriter : TextWriter
{
    private readonly ILogger _logger = logger ?? NullLogger.Instance;

    public virtual void Serialize(TData obj, TWriter stream)
    {
        LogSerialize(obj.GetType());
    }

    public virtual void OnBeforeChildSerialize(TData node, TData? previousNode, TWriter stream)
    {
        LogOnBeforeChildSerialize(node.GetType(), previousNode?.GetType());
    }

    public virtual void OnAfterChildSerialize(TData node, TData? nextNode, TWriter stream)
    {
        LogOnAfterChildSerialize(node.GetType(), nextNode?.GetType());
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

public partial class BaseStructDocNodeSerializer<TData>(ILogger? logger = null) : BaseStructDocNodeSerializer<TData, TextWriter>(logger)
    where TData : IStructDocNode
{

}
