using Microsoft.Extensions.DependencyInjection.Extensions;
using Sylvercode.SiteExtractor;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>Provides extension methods for <see cref="IServiceCollection"/> to register the <see cref="SiteExtractor"/> pipeline.</summary>
public static class SiteExtractorExtensions
{
    /// <summary>Registers <see cref="SiteExtractor"/> as a singleton <see cref="ISiteExtractor"/> and binds <see cref="SiteExtractorOptions"/> from configuration.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddSiteExtractor(this IServiceCollection services)
    {
        services.AddOptions<SiteExtractorOptions>().BindConfiguration(SiteExtractorOptions.SiteExtractor);
        services.TryAddSingleton<ISiteExtractor, SiteExtractor>();

        return services;
    }
}
