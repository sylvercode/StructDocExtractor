using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

/// <summary>Generic abstract base for all structural document node serializers, providing shared dispatch plumbing and a pluggable <see cref="Handler"/> slot.</summary>
/// <typeparam name="TData">The specific <see cref="IStructDocNode"/> type this serializer handles.</typeparam>
/// <typeparam name="TWriter">The specific <see cref="TextWriter"/> subtype used for output.</typeparam>
/// <remarks>
/// Separates the "what to write" logic (encapsulated in <see cref="Handler"/>) from the common dispatch
/// bookkeeping (logging, <see cref="NodeSerializationResult"/> creation, and type coercions).
/// Subclasses override or supply a <see cref="Handler"/> to provide actual serialization and
/// before/after-child callbacks without duplicating plumbing code.
/// Implements <see cref="IStructDocNodeSerializerServiceInit"/> so it self-reports its serializable type
/// for DI registration via <see cref="SerializerExtansions"/>.
/// </remarks>
public partial class BaseStructDocNodeSerializer<TData, TWriter>(
    BaseStructDocNodeSerializer<TData, TWriter>.Handler? handler = null,
    ILogger? logger = null)
    : IStructDocNodeSerializer<TData, TWriter>, ISerializer<TData, TWriter>,
    IStructDocNodeSerializerServiceInit
    where TData : IStructDocNode
    where TWriter : TextWriter
{

    /// <summary>Extensible handler that encapsulates the per-node write logic for <see cref="BaseStructDocNodeSerializer{TData,TWriter}"/>.</summary>
    public class Handler : IStructDocNodeSerializer<TData, TWriter>
    {
        /// <summary>Called before <paramref name="node"/> is serialized as a child; override to emit opening tokens or spacing.</summary>
        /// <param name="node">The node about to be serialized.</param>
        /// <param name="previousNode">The previous sibling, or <see langword="null"/> if first.</param>
        /// <param name="stream">The writer receiving output.</param>
        public virtual void OnBeforeAsChildSerialize(TData node, IStructDocNode? previousNode, TWriter stream)
        {
        }

        /// <summary>Performs the actual serialization of <paramref name="obj"/> into <paramref name="stream"/>; override to write node content.</summary>
        /// <param name="obj">The node to serialize.</param>
        /// <param name="stream">The writer receiving output.</param>
        /// <param name="result">The result object to update if content is directly emitted.</param>
        public virtual void Serialize(TData obj, TWriter stream, NodeSerializationResult result)
        {

        }

        /// <summary>Called after <paramref name="node"/> has been serialized as a child; override to emit closing tokens or trailing spacing.</summary>
        /// <param name="node">The node that was just serialized.</param>
        /// <param name="nextNode">The next sibling, or <see langword="null"/> if last.</param>
        /// <param name="stream">The writer receiving output.</param>
        public virtual void OnAfterAsChildSerialize(TData node, IStructDocNode? nextNode, TWriter stream)
        {
        }
    }

    /// <summary>Gets or sets the handler that provides the actual serialization logic for this serializer.</summary>
    protected Handler? Hdl { get; set; } = handler;

    private readonly ILogger _logger = logger ?? NullLogger.Instance;

    IEnumerable<Type> IStructDocNodeSerializerServiceInit.GetDefaultSeriazableType()
    {
        yield return typeof(TData);
    }

    /// <inheritdoc/>
    public NodeSerializationResult Serialize(TData obj, TWriter stream)
    {
        LogSerialize(obj.GetType());
        NodeSerializationResult result = new();
        Hdl?.Serialize(obj, stream, result);
        return result;
    }

    /// <inheritdoc/>
    public void OnBeforeAsChildSerialize(TData node, IStructDocNode? previousNode, TWriter stream)
    {
        LogOnBeforeChildSerialize(node.GetType(), previousNode?.GetType());
        Hdl?.OnBeforeAsChildSerialize(node, previousNode, stream);
    }

    /// <inheritdoc/>
    public void OnAfterAsChildSerialize(TData node, IStructDocNode? nextNode, TWriter stream)
    {
        LogOnAfterChildSerialize(node.GetType(), nextNode?.GetType());
        Hdl?.OnAfterAsChildSerialize(node, nextNode, stream);
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

/// <summary>Non-generic convenience base that fixes <c>TWriter</c> to <see cref="TextWriter"/> for serializers that do not need a specialized writer type.</summary>
/// <typeparam name="TData">The specific <see cref="IStructDocNode"/> type this serializer handles.</typeparam>
public abstract partial class BaseStructDocNodeSerializer<TData>(
    BaseStructDocNodeSerializer<TData, TextWriter>.Handler? handler = null,
     ILogger? logger = null)
    : BaseStructDocNodeSerializer<TData, TextWriter>(handler, logger)
    where TData : IStructDocNode
{

}
