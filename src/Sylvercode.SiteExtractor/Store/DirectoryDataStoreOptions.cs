using Microsoft.Extensions.Options;

namespace Sylvercode.SiteExtractor.Store;

public class DirectoryDataStoreOptions
{
    public const string DirectoryDataStore = nameof(DirectoryDataStore);
    public bool AutoCreateBaseDir { get; set; } = false;
    public static IOptions<DirectoryDataStoreOptions> NewOption() => Options.Create(new DirectoryDataStoreOptions());
}
