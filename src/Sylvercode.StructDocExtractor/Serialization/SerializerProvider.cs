using System.Collections;
using Microsoft.Extensions.Options;
using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;


public class SerializerProvider : ISerializerProvider, IEnumerable<KeyValuePair<Type, ISerializer>>
{
    public class SerializerCollection : IEnumerable<KeyValuePair<Type, ISerializer>>
    {
        internal readonly Dictionary<Type, ISerializer> _serializers = [];

        public void AddSerializer<TNode>(ISerializer serializer)
            => _serializers.TryAdd(typeof(TNode), serializer);

        public IEnumerator<KeyValuePair<Type, ISerializer>> GetEnumerator()
            => ((IEnumerable<KeyValuePair<Type, ISerializer>>)_serializers).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => ((IEnumerable)_serializers).GetEnumerator();
    }

    private readonly Dictionary<Type, ISerializer> _serializers = [];

    public SerializerProvider()
    {
    }

    public SerializerProvider(IOptions<SerializerCollection> serializers)
    {
        foreach (var serializer in serializers.Value)
            _serializers.TryAdd(serializer.Key, serializer.Value);
    }

    public ISerializer GetSerializerFor(IStructDocNode obj)
    {
        ArgumentNullException.ThrowIfNull(obj, nameof(obj));

        if (!_serializers.TryGetValue(obj.GetType(), out ISerializer? serializer))
            throw new InvalidOperationException($"No serializer found for type {obj.GetType().Name}");

        return serializer;
    }

    public void Add(Type type, ISerializer serializer)
    {
        if (!_serializers.TryAdd(type, serializer))
            throw new InvalidOperationException($"A serializer for type {type.Name} already exists");
    }

    #region IEnumerable<KeyValuePair<Type, ISerializer>>
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
