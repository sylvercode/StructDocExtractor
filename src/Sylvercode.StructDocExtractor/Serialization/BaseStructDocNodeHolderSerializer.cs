using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

/// <summary>Abstract base for holder node serializers, extending <see cref="BaseStructDocNodeSerializer{TData,TWriter}"/> with fine-grained child lifecycle callbacks.</summary>
/// <typeparam name="TData">The holder node type being serialized.</typeparam>
/// <typeparam name="TWriter">The specific <see cref="TextWriter"/> subtype used for output.</typeparam>
/// <typeparam name="TChild">The type of child nodes held by <typeparamref name="TData"/>.</typeparam>
/// <remarks>
/// Adds an inner <see cref="HolderHandler"/> that fires four lifecycle hooks — before the first child,
/// between siblings, after the last child, and when no children exist — allowing concrete serializers to
/// emit structural separators and container tokens without duplicating null-checking logic.
/// </remarks>
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
    /// <summary>Extensible handler providing child lifecycle hooks for <see cref="BaseStructDocNodeHolderSerializer{TData,TWriter,TChild}"/>.</summary>
    public class HolderHandler
    {
        /// <summary>Called just before the first child is serialized; override to emit an opening container token.</summary>
        /// <param name="parent">The holder node whose first child is about to be serialized.</param>
        /// <param name="firstChild">The first child node.</param>
        /// <param name="stream">The writer receiving output.</param>
        public virtual void OnBeforeFirstChildSerialize(TData parent, TChild firstChild, TWriter stream)
        {
        }

        /// <summary>Called between two sibling children; override to emit separators or spacing.</summary>
        /// <param name="parent">The holder node containing both siblings.</param>
        /// <param name="previousChild">The child just serialized.</param>
        /// <param name="nextChild">The child about to be serialized.</param>
        /// <param name="stream">The writer receiving output.</param>
        public virtual void OnBetweenSiblingSerialize(TData parent, TChild previousChild, TChild nextChild, TWriter stream)
        {
        }

        /// <summary>Called just after the last child is serialized; override to emit a closing container token.</summary>
        /// <param name="parent">The holder node whose last child was just serialized.</param>
        /// <param name="lastChild">The last child node.</param>
        /// <param name="stream">The writer receiving output.</param>
        public virtual void OnAfterLastChildSerialize(TData parent, TChild lastChild, TWriter stream)
        {
        }

        /// <summary>Called when the holder has no children; override to emit an empty-container representation.</summary>
        /// <param name="parent">The holder node with no children.</param>
        /// <param name="stream">The writer receiving output.</param>
        public virtual void OnNoChildSerialize(TData parent, TWriter stream)
        {
        }
    }

    /// <summary>Gets or sets the holder handler that provides child lifecycle callbacks for this serializer.</summary>
    protected HolderHandler? HolderHdl { get; set; } = holderHandler;

    private readonly ILogger _logger = logger ?? NullLogger.Instance;

    /// <inheritdoc/>
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

    /// <summary>Routes to <see cref="HolderHandler.OnBeforeFirstChildSerialize"/> and logs the event.</summary>
    protected virtual void OnBeforeFirstChildSerialize(TData parent, TChild nextChild, TWriter stream)
    {
        LogOnBeforeFirstChildSerialize(parent.GetType(), nextChild.GetType());
        HolderHdl?.OnBeforeFirstChildSerialize(parent, nextChild, stream);
    }

    /// <summary>Routes to <see cref="HolderHandler.OnBetweenSiblingSerialize"/> and logs the event.</summary>
    protected virtual void OnBetweenSiblingSerialize(TData parent, TChild previousChild, TChild nextChild, TWriter stream)
    {
        LogOnBetweenSiblingSerialize(parent.GetType(), previousChild.GetType(), nextChild.GetType());
        HolderHdl?.OnBetweenSiblingSerialize(parent, previousChild, nextChild, stream);
    }

    /// <summary>Routes to <see cref="HolderHandler.OnAfterLastChildSerialize"/> and logs the event.</summary>
    protected virtual void OnAfterLastChildSerialize(TData parent, TChild previousChild, TWriter stream)
    {
        LogOnAfterLastChildSerialize(parent.GetType(), previousChild.GetType());
        HolderHdl?.OnAfterLastChildSerialize(parent, previousChild, stream);
    }

    /// <summary>Routes to <see cref="HolderHandler.OnNoChildSerialize"/> and logs the event.</summary>
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

/// <summary>Non-generic convenience base that fixes <c>TWriter</c> to <see cref="TextWriter"/> for holder serializers that do not need a specialized writer type.</summary>
/// <typeparam name="TData">The holder node type being serialized.</typeparam>
/// <typeparam name="TChild">The type of child nodes held by <typeparamref name="TData"/>.</typeparam>
public abstract partial class BaseStructDocNodeHolderSerializer<TData, TChild>(
    BaseStructDocNodeHolderSerializer<TData, TextWriter, TChild>.HolderHandler? holderHandler = null,
    BaseStructDocNodeHolderSerializer<TData, TextWriter, TChild>.Handler? handler = null,
    ILogger? logger = null)
    : BaseStructDocNodeHolderSerializer<TData, TextWriter, TChild>(holderHandler, handler, logger)
    where TData : IStructDocNodeHolder<TChild>
    where TChild : class, IStructDocNode
{
}
