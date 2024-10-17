using System.Net;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.AngleSharp;
using Sylvercode.SiteExtractor.Sources;
using Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Extraction.PreviewProvider;
using Sylvercode.StructDocExtractor.StdHtml.Model;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class AngleSharpSiteExtractorExtentions
{
    private const string AngleSharpCookieColKey = nameof(AngleSharpCookieColKey);

    public static IServiceCollection AddAngleSharpWebSource(this IServiceCollection services)
    {
        services.AddMemoryCookieProvider();
        services.TryAddSingleton((sp) => AngleSharp.Configuration.Default.With(sp.GetRequiredService<MemoryCookieProvider>()));
        services.TryAddSingleton<IBrowsingContext, BrowsingContext>();
        services.TryAddSingleton<ISiteSource<INode>, AngleSharpWebSource>();

        return services;
    }

    private static IServiceCollection AddMemoryCookieProvider(this IServiceCollection services)
    {
        services.TryAddSingleton((sp) =>
        {
            MemoryCookieProvider provider = new();
            provider.Container.Add(sp.GetRequiredKeyedService<IOptions<CookieCollection>>(AngleSharpCookieColKey).Value);
            return provider;
        });
        services.TryAddSingleton((sp) => sp.GetRequiredService<MemoryCookieProvider>().Container);
        return services;
    }

    public static IServiceCollection ConfigreAngleSharpCookies<TDep>(this IServiceCollection services, Action<CookieCollection, TDep> configure)
        where TDep : class
    {
        services.AddOptions<CookieCollection>(AngleSharpCookieColKey).Configure(configure);
        return services;
    }

    public static IServiceCollection AddAngleSharpSiteExtractor(this IServiceCollection services, bool withDefaultSource = true, bool withImageCopier = true)
    {
        services.AddSiteExtractor()
            .AddResourceProcessorProvider()
            .AddResourceDataExtractorProcessor<INode>();

        if (withDefaultSource)
            // Must be added before AddHttpDownloader so its cookie container can be share.
            services.AddAngleSharpWebSource();

        if (withImageCopier)
        {
            services.AddImageCopierProcessor();
            if (withDefaultSource)
                services.AddHttpDownloader();
        }

        services.AddRouterExtractor<INode>();
        services.TryAddSingleton<IDataDiscriminatorFactory<INode, HtmlNodeDiscriminator>, HtmlNodeDiscriminatorFactory>();
        services.TryAddSingleton<IDataPreviewProvider<INode>, HtmlDataPreviewProvider>();

        return services;
    }
}
