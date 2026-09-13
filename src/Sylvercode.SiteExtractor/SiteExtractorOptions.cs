using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor;

/// <summary>Configuration options controlling the site extractor, including source authority, base path, and output directory.</summary>
/// <remarks>
/// Bound from the <c>"SiteExtractor"</c> configuration section via the options pattern.
/// Call <see cref="GetSourceBaseUri"/> and <see cref="GetOutputUri"/> to obtain typed <see cref="Uri"/> values
/// derived from the configured strings.
/// </remarks>
public class SiteExtractorOptions
{
    /// <summary>Gets the configuration section name used to bind these options.</summary>
    public const string SiteExtractor = nameof(SiteExtractor);

    /// <summary>Gets or sets the authority (scheme + host + optional port) of the source site.</summary>
    public string SourceAuthority { get; set; } = string.Empty;

    /// <summary>Gets or sets the base path on the source site from which extraction starts.</summary>
    /// <remarks>The value is normalised to a directory path on assignment.</remarks>
    public string SourceBasePath
    {
        get => field;
        set => field = value.AsDirPath();
    }

    /// <summary>Gets or sets the local output directory where extracted resources are written.</summary>
    /// <remarks>The value is normalised to a directory path on assignment.</remarks>
    public string OutputDirectory
    {
        get => field;
        set => field = value.AsDirPath();
    }

    /// <summary>Returns the absolute source base <see cref="Uri"/> combining <see cref="SourceAuthority"/> and <see cref="SourceBasePath"/>.</summary>
    /// <exception cref="InvalidOperationException"><see cref="SourceAuthority"/> has not been set.</exception>
    public Uri GetSourceBaseUri()
    {
        if (string.IsNullOrEmpty(SourceAuthority))
        {
            throw new InvalidOperationException("SourceAuthority is not set");
        }

        return new Uri(new Uri(SourceAuthority, UriKind.Absolute), SourceBasePath);
    }

    /// <summary>Returns a <see cref="Uri"/> representing the configured <see cref="OutputDirectory"/>.</summary>
    public Uri GetOutputUri() => new(OutputDirectory, UriKind.RelativeOrAbsolute);
}
