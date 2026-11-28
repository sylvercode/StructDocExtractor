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
    } = string.Empty;

    /// <summary>Gets or sets the local output directory where extracted resources are written.</summary>
    /// <remarks>The value is normalised to a directory path on assignment.</remarks>
    public string OutputDirectory
    {
        get => field;
        set => field = value.AsDirPath();
    } = string.Empty;

    /// <summary>Gets or sets the output roots of previously completed extractions whose manifests are required.</summary>
    /// <remarks>
    /// Each root identifies an earlier extraction through its existing <c>extraction-manifest.json</c>.
    /// Linked roots are not crawled or extracted again; their manifests are loaded and retained for catalog validation.
    /// </remarks>
    public IList<LinkedExtractionOptions> LinkedExtractions { get; set; } = [];

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

/// <summary>Identifies the output root of a previously completed extraction through its persisted manifest.</summary>
public class LinkedExtractionOptions
{
    /// <summary>Gets or sets the prior extraction output directory containing a valid <c>extraction-manifest.json</c>.</summary>
    /// <remarks>The directory is inspected for its manifest only; it is not used as a new extraction source or output.</remarks>
    public string OutputDirectory { get; set; } = string.Empty;
}
