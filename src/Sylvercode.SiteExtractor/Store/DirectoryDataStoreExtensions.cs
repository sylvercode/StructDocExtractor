using Sylvercode.SiteExtractor.Store;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class DirectoryDataStoreExtensions
{
    public static IServiceCollection AddDirectoryDataStore(this IServiceCollection services)
    {
        services.AddOptions<DirectoryDataStoreOptions>().BindConfiguration(nameof(DirectoryDataStore));
        services.AddSingleton<IDataStore, DirectoryDataStore>();

        return services;
    }
}
