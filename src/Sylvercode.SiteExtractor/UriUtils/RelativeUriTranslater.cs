using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Sylvercode.SiteExtractor.UriUtils;

/// <summary>Implementation of <see cref="IUriTranslater"/> that resolves a URI relative to a source base and optionally nests it under a configured output subdirectory</summary>
/// <remarks>
/// Converts extraction-context URIs to destination paths by making them relative to <c>SourceBaseUri</c>
/// and, when <c>outputPath</c> is non-empty, injecting that path as an intermediate directory segment.
/// This preserves the original directory structure while relocating resources to a new output root.
/// Accepts an optional <see cref="ILogger{TCategoryName}"/> to trace URI calculations at debug and trace levels.
/// </remarks>
public partial class RelativeUriTranslater(
    Uri SourceBaseUri,
    string outputPath,
    ILogger<RelativeUriTranslater>? logger) : IUriTranslater
{
    private readonly ILogger<RelativeUriTranslater> _logger = logger ?? NullLogger<RelativeUriTranslater>.Instance;

    private readonly Uri _outBaseDir = new(outputPath, UriKind.Relative);

    /// <summary>Translates <paramref name="uri"/> to a relative destination path, optionally nested under the configured output directory</summary>
    /// <param name="uri">The absolute source URI to translate</param>
    /// <returns>A relative <see cref="Uri"/> pointing to the resource's destination path</returns>
    public Uri Translate(Uri uri)
    {
        using var scope = _logger.BeginScope((
            OriginalUri: uri,
            SourceBaseUri,
            DestinationBaseUri: _outBaseDir
        ));

        Uri relativeUri = SourceBaseUri.MakeRelativeUri(uri);
        if (!string.IsNullOrEmpty(outputPath))
        {
            string fileName = Path.GetFileName(relativeUri.ToString());
            LogFileNameDestiantion(fileName);

            string dir = Path.GetDirectoryName(relativeUri.ToString()) ?? string.Empty;
            LogRelativeDirDestination(dir);

            relativeUri = new Uri(Path.Combine(dir, outputPath, fileName), UriKind.Relative);
        }

        LogRelativeTransation(relativeUri);
        return relativeUri;
    }

    [LoggerMessage(
        LogLevel.Debug,
        Message = "Relative translation result: {Result}")]
    private partial void LogRelativeTransation(Uri result);

    [LoggerMessage(
        LogLevel.Trace,
        Message = "Relative directory destination: {DirUri}")]
    private partial void LogRelativeDirDestination(string dirUri);

    [LoggerMessage(
        LogLevel.Trace,
        Message = "File name destination: {FileName}")]
    private partial void LogFileNameDestiantion(string fileName);
}
