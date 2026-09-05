using System.Net;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor;
using Sylvercode.SiteExtractor.Downloader.Http;
using Sylvercode.SiteExtractor.Sources;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>
/// Provides extension methods for <see cref="IServiceCollection"/> to register
/// <see cref="HttpDownloader"/> and its typed <see cref="System.Net.Http.HttpClient"/> in a DI container.
/// </summary>
public static class HttpDownloaderExtentions
{
    /// <summary>
    /// Registers <see cref="HttpDownloader"/> as the <c>ISiteSource&lt;byte[]&gt;</c> implementation,
    /// configuring a typed <see cref="System.Net.Http.HttpClient"/> with the base address from
    /// <see cref="Sylvercode.SiteExtractor.SiteExtractorOptions"/> and a cookie-aware message handler.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddHttpDownloader(this IServiceCollection services)
    {
        services.TryAddSingleton<CookieContainer>();
        services.AddHttpClient<ISiteSource<byte[]>, HttpDownloader>((sp, client) =>
            {
                var options = sp.GetRequiredService<IOptions<SiteExtractorOptions>>().Value;
                client.BaseAddress = options.GetSourceBaseUri();
            })
            .ConfigurePrimaryHttpMessageHandler((sp) => new HttpClientHandler
            {
                AllowAutoRedirect = true,
                UseCookies = true,
                CookieContainer = sp.GetRequiredService<CookieContainer>()
            })
            .RemoveAllLoggers();

        return services;
    }
}
