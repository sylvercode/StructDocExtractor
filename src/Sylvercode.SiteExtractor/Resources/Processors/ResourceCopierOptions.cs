using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Resources.Processors;

/// <summary>Configuration options for <see cref="ResourceCopier"/> controlling the output path and whether it is treated as absolute or relative.</summary>
public class ResourceCopierOptions
{
    private string outputPath = string.Empty;

    /// <summary>Gets or sets the output directory path where copied resources are written.</summary>
    /// <remarks>The value is normalised to a directory path on assignment via <c>AsDirPath()</c>.</remarks>
    public string OutputPath
    {
        get => outputPath;
        set => outputPath = value.AsDirPath();
    }

    /// <summary>Gets or sets a value indicating whether <see cref="OutputPath"/> is an absolute path on disk rather than relative to the source base URI.</summary>
    public bool IsOutputPathAbsolute { get; set; }
}
