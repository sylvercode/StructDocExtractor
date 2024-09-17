using Microsoft.Extensions.Options;

namespace Sylvercode.SiteExtractor.Store;

public class DirectoryDataStoreOptions : BaseDataStoreOptions
{
    public bool AutoCreateBaseDir { get; set; } = false;
    public static new IOptions<DirectoryDataStoreOptions> NewOption() => Options.Create(new DirectoryDataStoreOptions());
}
