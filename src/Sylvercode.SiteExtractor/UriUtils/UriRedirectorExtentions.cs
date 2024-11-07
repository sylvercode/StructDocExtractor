using System.Text.RegularExpressions;
using Sylvercode.SiteExtractor.UriUtils;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class UriRedirectorExtentions
{
    public static IServiceCollection AddUriRedirector(this IServiceCollection services, Regex regex, string replace)
    {
        services.AddSingleton<IUriRedirector>(new UriRedirector(regex, replace));
        return services;
    }
    public static IServiceCollection AddUriRedirector(this IServiceCollection services, string regex, string replace)
    {
        services.AddSingleton<IUriRedirector>(new UriRedirector(new Regex(regex), replace));
        return services;
    }
}
