using Microsoft.Extensions.DependencyInjection.Extensions;
using Sylvercode.StructDocExtractor.Model;
using Sylvercode.StructDocExtractor.Serialization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;

/// <summary>Provides extension methods for <see cref="IServiceCollection"/> to register the serialization pipeline and individual serializers.</summary>
public static class SerializerExtansions
{
    /// <summary>Registers the core serialization services (<see cref="IStructDocSerializer"/>, <see cref="ISerializerProvider"/>, and the serializer collection options) in <paramref name="services"/>.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddStructDocSerializer(this IServiceCollection services)
    {
        services.TryAddSingleton<IStructDocSerializer, StructDocSerializer>();
        services.AddOptions<SerializerProvider.SerializerCollection>();
        services.TryAddSingleton<ISerializerProvider, SerializerProvider>();

        return services;
    }

    /// <summary>Registers a typed <typeparamref name="TSerializer"/> for <typeparamref name="TNode"/> and adds it to the serializer collection.</summary>
    /// <typeparam name="TNode">The structural node type the serializer handles.</typeparam>
    /// <typeparam name="TSerializer">The serializer implementation type to register.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
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

    /// <summary>Registers a self-describing <typeparamref name="TSerializer"/> and wires its default serializable types into the serializer collection.</summary>
    /// <typeparam name="TSerializer">The serializer type implementing <see cref="IStructDocNodeSerializerServiceInit"/>.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddSerializer<TSerializer>(this IServiceCollection services)
        where TSerializer : class, IStructDocNodeSerializerServiceInit
    {
        services.TryAddSingleton<TSerializer>();
        return services.ConfigureSerializer<TSerializer>();
    }

    /// <summary>Adds an already-constructed <paramref name="serializer"/> to the serializer collection using its self-reported serializable types.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure.</param>
    /// <param name="serializer">The serializer instance to register.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection ConfigureSerializer(this IServiceCollection services, IStructDocNodeSerializerServiceInit serializer)
        => services.ConfigureSerializer(serializer, serializer.GetDefaultSeriazableType());

    /// <summary>Adds an already-constructed <paramref name="serializer"/> to the serializer collection for the specified <paramref name="seriazableType"/> types.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure.</param>
    /// <param name="serializer">The serializer instance to register.</param>
    /// <param name="seriazableType">The node types this serializer should handle.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection ConfigureSerializer(this IServiceCollection services, ISerializer serializer, IEnumerable<Type> seriazableType)
    {
        services.AddOptions<SerializerProvider.SerializerCollection>().Configure(col =>
            col.AddSerializer(seriazableType, serializer));
        return services;
    }

    /// <summary>Wires an already-registered <typeparamref name="TSerializer"/> singleton into the serializer collection using its self-reported serializable types.</summary>
    /// <typeparam name="TSerializer">The serializer type implementing <see cref="IStructDocNodeSerializerServiceInit"/>.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to configure.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection ConfigureSerializer<TSerializer>(this IServiceCollection services)
        where TSerializer : class, IStructDocNodeSerializerServiceInit
    {
        services.AddOptions<SerializerProvider.SerializerCollection>().Configure<TSerializer>((col, serializer) =>
            col.AddSerializer(serializer.GetDefaultSeriazableType(), serializer));
        return services;
    }
}
