using Microsoft.Extensions.DependencyInjection.Extensions;
using Sylvercode.SiteExtractor.Manifest;
using Sylvercode.SiteExtractor.Store;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>Provides registration helpers for in-memory extraction manifests.</summary>
public static class MemoryExtractionManifestStoreExtensions
{
    /// <summary>Optionally enables in-memory linked-extraction manifest support.</summary>
    /// <remarks>Do not register this store when the installation does not use linked extractions.</remarks>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddMemoryExtractionManifestStore(
        this IServiceCollection services)
    {
        services.TryAddSingleton<MemoryExtractionManifestStore>();
        services.TryAddSingleton<IExtractionManifestStore>(
            provider => provider.GetRequiredService<MemoryExtractionManifestStore>());
        services.TryAddSingleton<IExtractionManifestCatalog, ExtractionManifestCatalog>();
        return services;
    }
}
