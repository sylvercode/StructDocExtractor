using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sylvercode.SiteExtractor.Resources.Processors;
using Sylvercode.SiteExtractor.UriUtils;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class ResourceProcessorProviderExtentions
{
    public static IServiceCollection AddResourceProcessorProvider<TExtractionData>(this IServiceCollection services)
    {
        services.TryAddSingleton<IResourceDataExtractor<TExtractionData>, ResourceDataExtractor<TExtractionData>>();
        services.TryAddSingleton<IResourceCopiler, ResourceCopier>();
        services.AddSingleton<IResourceProcessorProvider>((serviceProvider) =>
        {
            // TODO Transfer to options!
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            //var options = configuration.Get<DDBOptions>();

            var resourceCopier = serviceProvider.GetRequiredService<IResourceCopiler>();
            var resourceDataExtractor = serviceProvider.GetRequiredService<IResourceDataExtractor<TExtractionData>>();

            serviceProvider.GetServices<IResourceProcessor>();

            ResourceProcessorProvider provider = new();
            provider.RegisterProcessor(ImageUriMatcher.Default, resourceCopier);
            provider.RegisterProcessor(new UriMatcherByBase(/*TODO*/new Uri("options.GetSiteUri()")), resourceDataExtractor);
            return provider;

        });

        return services;
    }
}
