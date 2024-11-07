using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor;

public class SiteExtractorOptions
{
    public const string SiteExtractor = nameof(SiteExtractor);

    public string SourceAuthority { get; set; } = string.Empty;

    private string _sourceBasePath = string.Empty;
    private string _outputDirectory = string.Empty;

    public string SourceBasePath
    {
        get => _sourceBasePath; set => _sourceBasePath = value.AsDirPath();
    }

    public string OutputDirectory
    {
        get => _outputDirectory;
        set => _outputDirectory = value.AsDirPath();
    }
    public Uri GetSourceBaseUri()
    {
        if (string.IsNullOrEmpty(SourceAuthority))
        {
            throw new InvalidOperationException("SourceAuthority is not set");
        }

        return new Uri(new Uri(SourceAuthority, UriKind.Absolute), SourceBasePath);
    }

    public Uri GetOutputUri() => new(OutputDirectory, UriKind.RelativeOrAbsolute);
}
