using Microsoft.Extensions.DependencyInjection.Extensions;
using Sylvercode.SiteExtractor.Markdown;
using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.UriUtils;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class MarkdownSerializationExtentions
{
    public static IServiceCollection AddMarkdownSerialization(this IServiceCollection services)
    {
        services.AddStructDocSerializer();
        services.TryAddSingleton<IUriTranslater, MarkdownUriTranslater>();
        services.TryAddSingleton<IReferencerUpdater, MarkdownReferencerUpdater>();

        return services;
    }
}
