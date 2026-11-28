using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Sylvercode.SiteExtractor.Manifest;

namespace Sylvercode.SiteExtractor.Store;

/// <summary>Abstract base providing common URI resolution and logging helpers to all <see cref="IDataStore"/> implementations.</summary>
/// <remarks>
/// Resolves relative output URIs against the base URI from <see cref="SiteExtractorOptions"/>.
/// Subclasses must implement <see cref="GetStream(Uri)"/> to supply their backing storage medium.
/// </remarks>
public abstract partial class BaseDataStore(
    IOptions<SiteExtractorOptions> options,
    ILogger? logger = null) : IDataStore
{
    private readonly ILogger _logger = logger ?? NullLogger.Instance;

    /// <inheritdoc/>
    public virtual Uri BaseUri => options.Value.GetOutputUri();

    /// <inheritdoc/>
    public abstract Stream GetStream(Uri uri);

    /// <summary>Returns a <see cref="StreamWriter"/> wrapping the stream for the specified URI.</summary>
    /// <param name="uri">The URI of the resource to write text to.</param>
    /// <returns>A <see cref="StreamWriter"/> for the stream at the given URI.</returns>
    public virtual StreamWriter GetStreamWriter(Uri uri) => new(GetStream(uri));

    /// <summary>Resolves a potentially relative URI to an absolute URI rooted at <see cref="BaseUri"/>.</summary>
    /// <param name="uri">The URI to resolve; may be relative or already absolute.</param>
    /// <returns>The fully resolved absolute URI under <see cref="BaseUri"/>.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the resolved URI is not a child of <see cref="BaseUri"/>, or when it targets
    /// the reserved root manifest file <see cref="ExtractionManifestValidator.ManifestFileName"/>.
    /// </exception>
    protected Uri GetCompleteUri(Uri uri)
    {
        if (uri.IsAbsoluteUri
            && BaseUri.IsBaseOf(uri))
        {
            LogUriAlreadyComplete(uri);
        }
        else
        {
            Uri completeUri = new(BaseUri, uri);
            if (!BaseUri.IsBaseOf(completeUri))
                throw new ArgumentException("Uri is not a child of the base uri", nameof(uri));

            LogUriCompleted(uri, completeUri);
        }

        DataStorePath path = ResolvePath(uri);
        string relativePath = path.RelativePath;
        if (ExtractionManifestValidator.IsReservedManifestFile(relativePath))
        {
            throw new ArgumentException(
                $"The output path '{ExtractionManifestValidator.ManifestFileName}' is reserved.",
                nameof(uri));
        }

        return path.Uri;
    }

    /// <summary>Resolves an output URI exactly once into a physical datastore path.</summary>
    /// <param name="uri">The URI accepted by <see cref="GetStream"/>.</param>
    /// <returns>The resolved URI, physical path, and portable relative path.</returns>
    /// <exception cref="ArgumentException">
    /// Thrown when the path escapes <see cref="BaseUri"/> or is empty.
    /// </exception>
    protected DataStorePath ResolvePath(Uri uri)
        => DataStorePathResolver.Resolve(BaseUri, uri);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Uri {Uri} is already complete")]
    private partial void LogUriAlreadyComplete(Uri uri);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Uri {OriginalUri} completd to {Uri}")]
    private partial void LogUriCompleted(Uri originalUri, Uri uri);
}
