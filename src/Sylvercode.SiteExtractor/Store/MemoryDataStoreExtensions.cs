using Sylvercode.SiteExtractor.Store;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>Provides extension methods for registering <see cref="MemoryDataStore"/> in a DI container.</summary>
public static class MemoryDataStoreExtensions
{
    /// <summary>Registers <see cref="MemoryDataStore"/> as a singleton <see cref="IDataStore"/> in <paramref name="services"/>.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddMemoryDataStore(this IServiceCollection services)
    {
        services.AddSingleton<MemoryDataStore>();
        services.AddSingleton<IDataStore>(provider => provider.GetRequiredService<MemoryDataStore>());
        return services;
    }
}
