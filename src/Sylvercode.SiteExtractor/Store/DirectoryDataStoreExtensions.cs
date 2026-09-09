using Sylvercode.SiteExtractor.Store;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

/// <summary>Provides extension methods for registering <see cref="DirectoryDataStore"/> in a DI container.</summary>
public static class DirectoryDataStoreExtensions
{
    /// <summary>Registers <see cref="DirectoryDataStore"/> as the singleton <see cref="IDataStore"/> in <paramref name="services"/>.</summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The same <paramref name="services"/> instance for chaining.</returns>
    public static IServiceCollection AddDirectoryDataStore(this IServiceCollection services)
    {
        services.AddSingleton<IDataStore, DirectoryDataStore>();

        return services;
    }
}
