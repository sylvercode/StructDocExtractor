using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;


public class SerializerProvider : ISerializerProvider
{
    private readonly Dictionary<Type, ISerializer> _serializers = [];

    public ISerializer GetSerializerFor(object obj)
    {
        ArgumentNullException.ThrowIfNull(obj);

        if (!_serializers.TryGetValue(obj.GetType(), out ISerializer? serializer))
            throw new InvalidOperationException($"No serializer found for type {obj.GetType().Name}");

        return serializer;
    }

    public void Add(Type type, ISerializer serializer)
    {
        if (!_serializers.TryAdd(type, serializer))
            throw new InvalidOperationException($"A serializer for type {type.Name} already exists");
    }
}
