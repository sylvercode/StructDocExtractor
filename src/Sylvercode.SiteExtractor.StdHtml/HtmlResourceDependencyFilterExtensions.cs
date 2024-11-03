using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sylvercode.SiteExtractor.Resources.Processors;

namespace Sylvercode.SiteExtractor.StdHtml;

public static class HtmlResourceDependencyFilterExtensions
{
    public static IServiceCollection AddHtmlResourceDependencyFilter(this IServiceCollection services)
    {
        services.TryAddSingleton<IResourceDependancyFilter, HtmlResourceDependencyFilter>();
        return services;
    }
}
