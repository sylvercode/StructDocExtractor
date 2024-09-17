using Sylvercode.SiteExtractor.Downloader.Http;
using Sylvercode.SiteExtractor.Sources;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class HttpDownloaderExtentions
{
    public static IServiceCollection AddHttpDownloader(this IServiceCollection services)
    {
        services.AddSingleton<ISiteSource<byte[]>, HttpDownloader>();

        return services;
    }
}
