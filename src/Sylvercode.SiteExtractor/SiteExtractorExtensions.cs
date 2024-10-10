using Microsoft.Extensions.DependencyInjection.Extensions;
using Sylvercode.SiteExtractor;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class SiteExtractorExtensions
{
    public static IServiceCollection AddSiteExtractor(this IServiceCollection services)
    {
        services.AddOptions<SiteExtractorOptions>().BindConfiguration(SiteExtractorOptions.SiteExtractor);
        services.TryAddSingleton<ISiteExtractor, SiteExtractor>();

        return services;
    }
}
