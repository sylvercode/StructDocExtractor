using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sylvercode.SiteExtractor.Resources.Processors;

namespace Sylvercode.SiteExtractor.StdHtml;

/// <summary>Provides extension methods for <see cref="IServiceCollection"/> to register <see cref="HtmlResourceDependencyFilter"/>.</summary>
public static class HtmlResourceDependencyFilterExtensions
{
    /// <summary>Registers <see cref="HtmlResourceDependencyFilter"/> as the singleton <see cref="IResourceDependancyFilter"/> if one has not already been registered.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddHtmlResourceDependencyFilter(this IServiceCollection services)
    {
        services.TryAddSingleton<IResourceDependancyFilter, HtmlResourceDependencyFilter>();
        return services;
    }
}
