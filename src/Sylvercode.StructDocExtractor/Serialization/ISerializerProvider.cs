using Sylvercode.StructDocExtractor.Model;

namespace Sylvercode.StructDocExtractor.Serialization;

/// <summary>Contract for resolving the correct <see cref="ISerializer"/> for a given structural node.</summary>
public interface ISerializerProvider
{
    /// <summary>Returns the serializer registered for the runtime type of <paramref name="obj"/>.</summary>
    /// <param name="obj">The structural document node whose serializer is needed.</param>
    /// <returns>The <see cref="ISerializer"/> registered for <paramref name="obj"/>'s concrete type.</returns>
    ISerializer GetSerializerFor(IStructDocNode obj);
}
