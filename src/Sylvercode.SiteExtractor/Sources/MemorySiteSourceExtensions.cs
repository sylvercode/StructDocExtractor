using Sylvercode.SiteExtractor.Sources;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class MemorySiteSourceExtensions
{
    public static IServiceCollection AddMemorySiteSource<TData>(this IServiceCollection services)
    {
        services.AddOptions<MemorySiteSource<TData>.Options>();
        services.AddSingleton<MemorySiteSource<TData>>();
        services.AddSingleton<ISiteSource<TData>>(provider => provider.GetRequiredService<MemorySiteSource<TData>>());
        return services;
    }
}
