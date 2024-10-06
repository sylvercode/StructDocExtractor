using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor;

public class SiteExtractorOptions
{
    public const string SiteExtractor = nameof(SiteExtractor);


    public string SourceAuthority { get; set; } = string.Empty;

    private string sourceBasePath = string.Empty;
    private string outputDirectory = string.Empty;

    public string SourceBasePath
    {
        get => sourceBasePath; set => sourceBasePath = value.AsDirPath();
    }

    public string OutputDirectory
    {
        get => outputDirectory;
        set => outputDirectory = value.AsDirPath();
    }
    public Uri GetSourceBaseUri()
    {
        if (string.IsNullOrEmpty(SourceAuthority))
        {
            throw new InvalidOperationException("SourceAuthority is not set");
        }

        return new Uri(new Uri(SourceAuthority, UriKind.Absolute), SourceBasePath);
    }
}
