namespace Sylvercode.SiteExtractor;

public class SiteExtractorOptions
{
    public const string SiteExtractor = nameof(SiteExtractor);
    public string SourceAuthority { get; set; } = string.Empty;
    public string SourceBasePath { get; set; } = string.Empty;
    public string OutputDirectory { get; set; } = string.Empty;

    public Uri GetSourceBaseUri()
    {
        if (string.IsNullOrEmpty(SourceAuthority))
        {
            throw new InvalidOperationException("SourceAuthority is not set");
        }

        return new Uri(new Uri(SourceAuthority, UriKind.Absolute), SourceBasePath);
    }
}
