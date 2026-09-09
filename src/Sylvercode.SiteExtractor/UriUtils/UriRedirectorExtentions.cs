using System.Text.RegularExpressions;
using Sylvercode.SiteExtractor.UriUtils;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>Provides extension methods for <see cref="IServiceCollection"/> to register <see cref="IUriRedirector"/> instances</summary>
public static class UriRedirectorExtentions
{
    /// <summary>Registers a <see cref="UriRedirector"/> as a singleton <see cref="IUriRedirector"/> using a compiled <see cref="Regex"/></summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to</param>
    /// <param name="regex">The compiled regular expression applied to URI strings</param>
    /// <param name="replace">The replacement string used when the regex matches</param>
    /// <returns>The same <paramref name="services"/> instance for chaining</returns>
    public static IServiceCollection AddUriRedirector(this IServiceCollection services, Regex regex, string replace)
    {
        services.AddSingleton<IUriRedirector>(new UriRedirector(regex, replace));
        return services;
    }

    /// <summary>Registers a <see cref="UriRedirector"/> as a singleton <see cref="IUriRedirector"/> using a pattern string</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to</param>
    /// <param name="regex">The regular expression pattern string applied to URI strings</param>
    /// <param name="replace">The replacement string used when the regex matches</param>
    /// <returns>The same <paramref name="services"/> instance for chaining</returns>
    public static IServiceCollection AddUriRedirector(this IServiceCollection services, string regex, string replace)
    {
        services.AddSingleton<IUriRedirector>(new UriRedirector(new Regex(regex), replace));
        return services;
    }
}
