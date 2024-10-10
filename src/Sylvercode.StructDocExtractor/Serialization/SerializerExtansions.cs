using Microsoft.Extensions.DependencyInjection.Extensions;
using Sylvercode.StructDocExtractor.Serialization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class SerializerExtansions
{
    public static IServiceCollection AddStructDocSerializer(this IServiceCollection services)
    {
        services.TryAddSingleton<IStructDocSerializer, StructDocSerializer>();
        services.AddOptions<SerializerProvider.SerializerCollection>();
        services.TryAddSingleton<ISerializerProvider, SerializerProvider>();

        return services;
    }

    public static IServiceCollection AddSerializer<TNode, TSerializer>(this IServiceCollection services)
        where TSerializer : class, ISerializer
    {
        services.TryAddSingleton<TSerializer>();
        services.AddOptions<SerializerProvider.SerializerCollection>().Configure<TSerializer>((col, serializer) =>
            col.AddSerializer<TNode>(serializer)
        );

        return services;
    }
}
