using Sylvercode.SiteExtractor.Store;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace Microsoft.Extensions.DependencyInjection;
#pragma warning restore IDE0130 // Namespace does not match folder structure

public static class MemoryDataStoreExtensions
{
    public static IServiceCollection AddMemoryDataStore(this IServiceCollection services)
    {
        services.AddSingleton<MemoryDataStore>();
        services.AddSingleton<IDataStore>(provider => provider.GetRequiredService<MemoryDataStore>());
        return services;
    }
}
