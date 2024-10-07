using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sylvercode.SiteExtractor;
using Sylvercode.SiteExtractor.AngleSharp;
using Sylvercode.SiteExtractor.Sources;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class AngleSharpSiteExtractorExtentions
{
    public static IServiceCollection AddAngleSharpWebSource(this IServiceCollection services)
    {
        services.TryAddSingleton<MemoryCookieProvider>();
        services.TryAddSingleton((sp) => sp.GetRequiredService<MemoryCookieProvider>().Container);
        services.TryAddSingleton((sp) => AngleSharp.Configuration.Default.With(sp.GetRequiredService<MemoryCookieProvider>()));
        services.TryAddSingleton<IBrowsingContext, BrowsingContext>();
        services.AddSingleton<ISiteSource<IElement>, AngleSharpWebSource>();

        return services;
    }

    public static IServiceCollection AddAngleSharpSiteExtractor(this IServiceCollection services)
    {
        services.AddSingleton<ISiteExtractor, SiteExtractor>();

        return services;
    }
}
