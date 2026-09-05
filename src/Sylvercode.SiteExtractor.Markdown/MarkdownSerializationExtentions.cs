using Microsoft.Extensions.DependencyInjection.Extensions;
using Sylvercode.SiteExtractor.Markdown;
using Sylvercode.SiteExtractor.Resources;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>Provides extension methods for <see cref="IServiceCollection"/> to register all Markdown serialization and site-extraction services.</summary>
public static class MarkdownSerializationExtentions
{
    /// <summary>Registers the Markdown serializer pipeline, stream-writer provider, URI translater, and referencer updater in <paramref name="services"/>.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddMarkdownSerialization(this IServiceCollection services)
    {
        services.AddStructDocSerializer();
        services.AddMarkdownStreamWriterProvider();
        services.TryAddSingleton<IResourceUriTranslater, MarkdownUriTranslater>();
        services.TryAddSingleton<IReferencerUpdater, MarkdownReferencerUpdater>();
        services.AddMarkdownSerializers();

        return services;
    }
}
