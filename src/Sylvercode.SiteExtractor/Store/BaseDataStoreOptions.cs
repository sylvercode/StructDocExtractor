using Microsoft.Extensions.Options;

namespace Sylvercode.SiteExtractor.Store;

public class BaseDataStoreOptions
{
    public string BaseUri { get; set; } = string.Empty;
    public static IOptions<BaseDataStoreOptions> NewOption() => Options.Create(new BaseDataStoreOptions());
}
