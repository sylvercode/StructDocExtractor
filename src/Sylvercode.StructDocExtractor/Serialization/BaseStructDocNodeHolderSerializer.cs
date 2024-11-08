using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

public partial class BaseStructDocNodeHolderSerializer<TData, TWriter, TChild>(
    BaseStructDocNodeHolderSerializer<TData, TWriter, TChild>.HolderHandler? holderHandler = null,
    BaseStructDocNodeHolderSerializer<TData, TWriter, TChild>.Handler? handler = null,
    ILogger? logger = null)
    : BaseStructDocNodeSerializer<TData, TWriter>(handler, logger),
    IStructDocNodeHolderSerializer<TData, TWriter, TChild>
    where TData : IStructDocNodeHolder<TChild>
    where TWriter : TextWriter
    where TChild : class, IStructDocNode
{
    public class HolderHandler
    {
        public virtual void OnBeforeFirstChildSerialize(TData parent, TChild firstChild, TWriter stream)
        {
        }

        public virtual void OnBetweenSiblingSerialize(TData parent, TChild previousChild, TChild nextChild, TWriter stream)
        {
        }

        public virtual void OnAfterLastChildSerialize(TData parent, TChild lastChild, TWriter stream)
        {
        }

        public virtual void OnNoChildSerialize(TData parent, TWriter stream)
        {
        }
    }

    protected HolderHandler? HolderHdl { get; set; } = holderHandler;

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
        HolderHdl?.OnBeforeFirstChildSerialize(parent, nextChild, stream);
    }

    protected virtual void OnBetweenSiblingSerialize(TData parent, TChild previousChild, TChild nextChild, TWriter stream)
    {
        LogOnBetweenSiblingSerialize(parent.GetType(), previousChild.GetType(), nextChild.GetType());
        HolderHdl?.OnBetweenSiblingSerialize(parent, previousChild, nextChild, stream);
    }

    protected virtual void OnAfterLastChildSerialize(TData parent, TChild previousChild, TWriter stream)
    {
        LogOnAfterLastChildSerialize(parent.GetType(), previousChild.GetType());
        HolderHdl?.OnAfterLastChildSerialize(parent, previousChild, stream);
    }

    protected virtual void OnNoChildSerialize(TData parent, TWriter stream)
    {
        LogOnNoChildSerialize(parent.GetType());
        HolderHdl?.OnNoChildSerialize(parent, stream);
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

public abstract partial class BaseStructDocNodeHolderSerializer<TData, TChild>(
    BaseStructDocNodeHolderSerializer<TData, TextWriter, TChild>.HolderHandler? holderHandler = null,
    BaseStructDocNodeHolderSerializer<TData, TextWriter, TChild>.Handler? handler = null,
    ILogger? logger = null)
    : BaseStructDocNodeHolderSerializer<TData, TextWriter, TChild>(holderHandler, handler, logger)
    where TData : IStructDocNodeHolder<TChild>
    where TChild : class, IStructDocNode
{
}
