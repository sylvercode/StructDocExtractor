using Microsoft.Extensions.DependencyInjection.Extensions;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;

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
        where TSerializer : class, ISerializer<TNode>
        where TNode : IStructDocNode
    {
        services.TryAddSingleton<TSerializer>();
        services.AddOptions<SerializerProvider.SerializerCollection>().Configure<TSerializer>((col, serializer) =>
            col.AddSerializer<TNode>(serializer)
        );

        return services;
    }

    public static IServiceCollection AddSerializer<TSerializer>(this IServiceCollection services)
        where TSerializer : class, IStructDocNodeSerializerServiceInit
    {
        services.TryAddSingleton<TSerializer>();
        return services.ConfigureSerializer<TSerializer>();
    }

    public static IServiceCollection ConfigureSerializer(this IServiceCollection services, IStructDocNodeSerializerServiceInit serializer)
        => services.ConfigureSerializer(serializer, serializer.GetDefaultSeriazableType());

    public static IServiceCollection ConfigureSerializer(this IServiceCollection services, ISerializer serializer, IEnumerable<Type> seriazableType)
    {
        services.AddOptions<SerializerProvider.SerializerCollection>().Configure(col =>
            col.AddSerializer(seriazableType, serializer));
        return services;
    }

    public static IServiceCollection ConfigureSerializer<TSerializer>(this IServiceCollection services)
        where TSerializer : class, IStructDocNodeSerializerServiceInit
    {
        services.AddOptions<SerializerProvider.SerializerCollection>().Configure<TSerializer>((col, serializer) =>
            col.AddSerializer(serializer.GetDefaultSeriazableType(), serializer));
        return services;
    }
}
