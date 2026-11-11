using System.Net;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor;
using Sylvercode.SiteExtractor.UriUtils;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>Provides extension methods for registering HTTP redirect resolution.</summary>
public static class HttpUriRedirectorExtentions
{
    /// <summary>Registers an <see cref="HttpUriRedirector"/> that inspects server redirect responses.</summary>
    /// <param name="services">The service collection to add the resolver to.</param>
    /// <returns>The same service collection for chaining.</returns>
    public static IServiceCollection AddHttpUriRedirector(this IServiceCollection services)
    {
        services.TryAddSingleton<CookieContainer>();
        services.AddHttpClient<IUriRedirector, HttpUriRedirector>((serviceProvider, client) =>
            {
                var options = serviceProvider.GetRequiredService<IOptions<SiteExtractorOptions>>().Value;
                client.BaseAddress = options.GetSourceBaseUri();
                client.Timeout = TimeSpan.FromSeconds(15);
            })
            .ConfigurePrimaryHttpMessageHandler(serviceProvider => new HttpClientHandler
            {
                AllowAutoRedirect = false,
                UseCookies = true,
                CookieContainer = serviceProvider.GetRequiredService<CookieContainer>()
            })
            .RemoveAllLoggers();

        return services;
    }
}
