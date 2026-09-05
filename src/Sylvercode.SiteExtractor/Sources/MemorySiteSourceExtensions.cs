using Sylvercode.SiteExtractor.Sources;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>Provides extension methods for registering <see cref="MemorySiteSource{TData}"/> in a DI container.</summary>
public static class MemorySiteSourceExtensions
{
    /// <summary>Registers <see cref="MemorySiteSource{TData}"/> and its <see cref="ISiteSource{TData}"/> binding as singletons in <paramref name="services"/>.</summary>
    /// <typeparam name="TData">The type of document data the source provides.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddMemorySiteSource<TData>(this IServiceCollection services)
    {
        services.AddOptions<MemorySiteSource<TData>.Options>();
        services.AddSingleton<MemorySiteSource<TData>>();
        services.AddSingleton<ISiteSource<TData>>(provider => provider.GetRequiredService<MemorySiteSource<TData>>());
        return services;
    }
}
