using Microsoft.Extensions.DependencyInjection.Extensions;
using Sylvercode.StructDocExtractor.Serialization;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class MarkdownSerializerProviderExtentions
{
    public static IServiceCollection AddMarkdownSerializerProvider(this IServiceCollection services)
    {
        // TODO: Add ISerializerProvider
        services.TryAddSingleton<IStructDocSerializer, StructDocSerializer>();

        return services;
    }
}
