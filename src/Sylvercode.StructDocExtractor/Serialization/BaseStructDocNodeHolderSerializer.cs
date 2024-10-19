using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public abstract partial class BaseStructDocNodeHolderSerializer<TData, TWriter, TChild>(ILogger? logger = null) : BaseStructDocNodeSerializer<TData, TWriter>, IStructDocNodeHolderSerializer<TData, TWriter, TChild>
    where TData : IStructDocNodeHolder<TChild>
    where TWriter : TextWriter
    where TChild : class, IStructDocNode
{
    private readonly ILogger _logger = logger ?? NullLogger.Instance;
    public virtual void OnBetweenChildrenSerialize(TData parent, TChild? previousChild, TChild? nextChild, TWriter stream)
    {
        if (previousChild is null)
        {
            if (nextChild is null)
                OnNoChildSerialize(parent, stream);
            else
                OnBeforeFirstChildSerialize(parent, nextChild, stream);
        }
        else
        {
            if (nextChild is null)
                OnAfterLastChildSerialize(parent, previousChild, stream);
            else
                OnBetweenSiblingSerialize(parent, previousChild, nextChild, stream);
        }
    }

    protected virtual void OnBeforeFirstChildSerialize(TData parent, TChild nextChild, TWriter stream)
    {
        LogOnBeforeFirstChildSerialize(parent.GetType(), nextChild.GetType());
    }

    protected virtual void OnBetweenSiblingSerialize(TData parent, TChild previousChild, TChild nextChild, TWriter stream)
    {
        LogOnBetweenSiblingSerialize(parent.GetType(), previousChild.GetType(), nextChild.GetType());
    }

    protected virtual void OnAfterLastChildSerialize(TData parent, TChild previousChild, TWriter stream)
    {
        LogOnAfterLastChildSerialize(parent.GetType(), previousChild.GetType());
    }

    protected virtual void OnNoChildSerialize(TData parent, TWriter stream)
    {
        LogOnNoChildSerialize(parent.GetType());
    }

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Before first child serialize for data type {dataType} with next child data type {nextChildDataType}")]
    private partial void LogOnBeforeFirstChildSerialize(Type dataType, Type nextChildDataType);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Between sibling serialize for data type {dataType} with previous child data type {previousChildDataType} and next child data type {nextChildDataType}")]
    private partial void LogOnBetweenSiblingSerialize(Type dataType, Type previousChildDataType, Type nextChildDataType);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "After last child serialize for data type {dataType} with previous child data type {previousChildDataType}")]
    private partial void LogOnAfterLastChildSerialize(Type dataType, Type previousChildDataType);

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "No child serialize for data type {dataType}")]
    private partial void LogOnNoChildSerialize(Type dataType);
}

public abstract partial class BaseStructDocNodeHolderSerializer<TData, TChild>(ILogger? logger = null)
    : BaseStructDocNodeHolderSerializer<TData, TextWriter, TChild>(logger)
    where TData : IStructDocNodeHolder<TChild>
    where TChild : class, IStructDocNode
{
}
