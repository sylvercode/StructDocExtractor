using System.Collections;
using Microsoft.Extensions.Options;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;


/// <summary>Default <see cref="ISerializerProvider"/> that resolves serializers by exact runtime node type.</summary>
/// <remarks>
/// Maintains an internal dictionary keyed by <see cref="Type"/>. Serializers can be registered programmatically
/// via <see cref="Add"/> or populated from DI via <see cref="IOptions{SerializerCollection}"/>.
/// Throws <see cref="InvalidOperationException"/> if no serializer is registered for a requested type,
/// enforcing that all serializable node types have explicit bindings.
/// </remarks>
public class SerializerProvider : ISerializerProvider, IEnumerable<KeyValuePair<Type, ISerializer>>
{
    /// <summary>A configurable dictionary-backed registry of <see cref="ISerializer"/> instances keyed by node type, used to populate <see cref="SerializerProvider"/> via DI options.</summary>
    public class SerializerCollection : IEnumerable<KeyValuePair<Type, ISerializer>>
    {
        internal readonly Dictionary<Type, ISerializer> _serializers = [];

        /// <summary>Registers <paramref name="serializer"/> for the <typeparamref name="TNode"/> node type.</summary>
        /// <typeparam name="TNode">The node type to associate with this serializer.</typeparam>
        /// <param name="serializer">The serializer to register.</param>
        public void AddSerializer<TNode>(ISerializer serializer)
            => AddSerializer(typeof(TNode), serializer);

        /// <summary>Registers <paramref name="serializer"/> for the specified <paramref name="type"/>.</summary>
        /// <param name="type">The node type to associate with this serializer.</param>
        /// <param name="serializer">The serializer to register.</param>
        public void AddSerializer(Type type, ISerializer serializer)
            => _serializers.TryAdd(type, serializer);

        /// <summary>Registers <paramref name="serializer"/> for each type in <paramref name="types"/>.</summary>
        /// <param name="types">The node types to associate with this serializer.</param>
        /// <param name="serializer">The serializer to register.</param>
        public void AddSerializer(IEnumerable<Type> types, ISerializer serializer)
        {
            foreach (var type in types)
                _serializers.TryAdd(type, serializer);
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<Type, ISerializer>> GetEnumerator()
            => ((IEnumerable<KeyValuePair<Type, ISerializer>>)_serializers).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable)_serializers).GetEnumerator();
    }

    private readonly Dictionary<Type, ISerializer> _serializers = [];

    /// <summary>Initializes a new empty <see cref="SerializerProvider"/>.</summary>
    public SerializerProvider()
    {
    }

    /// <summary>Initializes a new <see cref="SerializerProvider"/> pre-populated from the DI options collection.</summary>
    /// <param name="serializers">The options collection of serializers registered via <see cref="SerializerCollection"/>.</param>
    public SerializerProvider(IOptions<SerializerCollection> serializers)
    {
        foreach (var serializer in serializers.Value)
            _serializers.TryAdd(serializer.Key, serializer.Value);
    }

    /// <inheritdoc/>
    /// <exception cref="InvalidOperationException">Thrown if no serializer is registered for <paramref name="obj"/>'s runtime type.</exception>
    public ISerializer GetSerializerFor(IStructDocNode obj)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));

        if (!_serializers.TryGetValue(obj.GetType(), out ISerializer? serializer))
            throw new InvalidOperationException($"No serializer found for type {obj.GetType().Name}");

        return serializer;
    }

    /// <summary>Programmatically registers <paramref name="serializer"/> for the given <paramref name="type"/>.</summary>
    /// <param name="type">The node type to associate with this serializer.</param>
    /// <param name="serializer">The serializer to register.</param>
    /// <exception cref="InvalidOperationException">Thrown if a serializer for <paramref name="type"/> is already registered.</exception>
    public void Add(Type type, ISerializer serializer)
    {
        if (!_serializers.TryAdd(type, serializer))
            throw new InvalidOperationException($"A serializer for type {type.Name} already exists");
    }

    #region IEnumerable<KeyValuePair<Type, ISerializer>>
    /// <inheritdoc/>
    public IEnumerator<KeyValuePair<Type, ISerializer>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<Type, ISerializer>>)_serializers).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_serializers).GetEnumerator();
    }
    #endregion
}
