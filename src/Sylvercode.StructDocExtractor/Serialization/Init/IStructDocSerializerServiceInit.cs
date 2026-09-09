using Sylvercode.StructDocExtractor.Serialization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>Contract for serializers that self-register their default serializable node types with the DI pipeline.</summary>
public interface IStructDocNodeSerializerServiceInit : ISerializer
{
    /// <summary>Returns the node types this serializer handles by default.</summary>
    /// <returns>An enumerable of <see cref="Type"/> values representing the serializable node types.</returns>
    IEnumerable<Type> GetDefaultSeriazableType();
}
