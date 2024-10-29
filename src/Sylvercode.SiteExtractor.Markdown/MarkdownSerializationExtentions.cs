using Microsoft.Extensions.DependencyInjection.Extensions;
using Sylvercode.SiteExtractor.Markdown;
using Sylvercode.SiteExtractor.Resources;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class MarkdownSerializationExtentions
{
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
