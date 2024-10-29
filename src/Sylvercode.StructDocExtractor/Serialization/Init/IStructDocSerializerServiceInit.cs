using Sylvercode.StructDocExtractor.Serialization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public interface IStructDocNodeSerializerServiceInit : ISerializer
{
    IEnumerable<Type> GetDefaultSeriazableType();
}
