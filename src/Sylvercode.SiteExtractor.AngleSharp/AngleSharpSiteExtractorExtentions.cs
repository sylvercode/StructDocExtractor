using System.Net;
using AngleSharp;
using AngleSharp.Dom;
using AngleSharp.Io;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.AngleSharp;
using Sylvercode.SiteExtractor.Sources;
using Sylvercode.SiteExtractor.StdHtml;
using Sylvercode.StructDocExtractor.AngleSharp.Extraction.Factory;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Extraction.PreviewProvider;
using Sylvercode.StructDocExtractor.StdHtml.Model;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to register the AngleSharp-backed
/// site source and full extraction pipeline into a DI container.
/// </summary>
public static class AngleSharpSiteExtractorExtentions
{
    private const string AngleSharpCookieColKey = nameof(AngleSharpCookieColKey);

    /// <summary>
    /// Registers <see cref="AngleSharpWebSource"/> as the <see cref="ISiteSource{TData}"/> singleton
    /// for live HTTP fetching, along with its shared <see cref="MemoryCookieProvider"/> and
    /// <see cref="IBrowsingContext"/> dependencies.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddAngleSharpWebSource(this IServiceCollection services)
    {
        services.AddMemoryCookieProvider();
        services.TryAddSingleton((sp) =>
            AngleSharp.Configuration.Default
                .With(sp.GetRequiredService<MemoryCookieProvider>())
                .WithRequesters()
                .WithDefaultLoader());
        services.TryAddSingleton<IBrowsingContext, BrowsingContext>();
        services.TryAddSingleton<ISiteSource<INode>, AngleSharpWebSource>();

        return services;
    }

    private static IServiceCollection AddMemoryCookieProvider(this IServiceCollection services)
    {
        services.TryAddSingleton((sp) =>
        {
            MemoryCookieProvider provider = new();
            provider.Container.Add(sp.GetRequiredService<IOptionsMonitor<CookieCollection>>().Get(AngleSharpCookieColKey));
            return provider;
        });
        services.TryAddSingleton((sp) => sp.GetRequiredService<MemoryCookieProvider>().Container);
        return services;
    }

    /// <summary>
    /// Configures the AngleSharp cookie collection used by <see cref="AngleSharpWebSource"/> by
    /// applying a delegate that populates the collection from a resolved dependency.
    /// </summary>
    /// <typeparam name="TDep">The dependency type resolved from the DI container to assist cookie configuration.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add options to.</param>
    /// <param name="configure">Delegate that receives the <see cref="CookieCollection"/> and <typeparamref name="TDep"/> to configure.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection ConfigreAngleSharpCookies<TDep>(this IServiceCollection services, Action<CookieCollection, TDep> configure)
        where TDep : class
    {
        services.AddOptions<CookieCollection>(AngleSharpCookieColKey).Configure(configure);
        return services;
    }

    /// <summary>
    /// Registers the complete AngleSharp-backed site extractor pipeline, including the resource
    /// processor provider, HTML dependency filter, optional live web source, and optional image copier.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="withDefaultSource">
    /// When <see langword="true"/> (default), also registers <see cref="AngleSharpWebSource"/> as the
    /// live <see cref="ISiteSource{TData}"/>. Must be <see langword="true"/> when using
    /// <paramref name="withImageCopier"/> so the cookie container is shared.
    /// </param>
    /// <param name="withImageCopier">
    /// When <see langword="true"/> (default), registers the image copier processor and HTTP downloader
    /// alongside the extractor.
    /// </param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddAngleSharpSiteExtractor(this IServiceCollection services, bool withDefaultSource = true, bool withImageCopier = true)
    {
        services.AddSiteExtractor()
            .AddResourceProcessorProvider()
            .AddResourceDataExtractorProcessor<INode>()
            .AddHtmlResourceDependencyFilter();

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
