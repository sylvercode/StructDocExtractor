using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Sylvercode.SiteExtractor.AngleSharp;
using Sylvercode.SiteExtractor.Sources;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class AngleSharpWebSourceExtentions
{
    public static IServiceCollection AddAngleSharpWebSource(this IServiceCollection services)
    {
        services.TryAddSingleton(AngleSharp.Configuration.Default);
        services.TryAddSingleton<IBrowsingContext, BrowsingContext>();
        services.AddSingleton<ISiteSource<IElement>, AngleSharpWebSource>();

        return services;
    }
}
