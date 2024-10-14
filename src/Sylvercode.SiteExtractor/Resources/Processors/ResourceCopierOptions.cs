using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Resources.Processors;

public class ResourceCopierOptions
{
    private string outputPath = string.Empty;
    public string OutputPath
    {
        get => outputPath;
        set => outputPath = value.AsDirPath();
    }
    public bool IsOutputPathAbsolute { get; set; }
}
