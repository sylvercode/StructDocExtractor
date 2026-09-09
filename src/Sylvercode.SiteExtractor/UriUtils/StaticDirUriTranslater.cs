using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Sylvercode.SiteExtractor.UriUtils;

/// <summary>Implementation of <see cref="IUriTranslater"/> that maps a URI to a flat path inside a configured static output directory</summary>
/// <remarks>
/// Strips all directory hierarchy from the source URI, keeping only the file name, and combines it
/// with the configured <c>outputPath</c> to produce a relative output URI.
/// This flattens resources from varying URI depths into a single directory, which is useful for
/// static asset storage during site extraction.
/// Accepts an optional <see cref="ILogger{TCategoryName}"/> for observability at debug and trace levels.
/// </remarks>
public partial class StaticDirUriTranslater(
    string outputPath,
    ILogger<StaticDirUriTranslater>? logger) : IUriTranslater
{
    private readonly ILogger<StaticDirUriTranslater> _logger = logger ?? NullLogger<StaticDirUriTranslater>.Instance;

    /// <summary>Translates <paramref name="uri"/> to a relative path under the configured output directory, using only the file name</summary>
    /// <param name="uri">The source URI to translate; may be absolute or relative</param>
    /// <returns>A relative <see cref="Uri"/> of the form <c>outputPath/filename</c></returns>
    public Uri Translate(Uri uri)
    {
        using var scope = _logger.BeginScope((
            OriginalUri: uri,
            DestinationBaseUri: outputPath
        ));

        string path = uri.IsAbsoluteUri ? uri.AbsolutePath : uri.ToString();
        string fileName = Path.GetFileName(path);
        LogFileNameDestiantion(fileName);

        Uri relativeResult = new(Path.Combine(outputPath, fileName), UriKind.Relative);
        LogAbsoluteTranslation(relativeResult);
        return relativeResult;
    }

    [LoggerMessage(
        LogLevel.Debug,
        Message = "Absolute translation result: {Result}")]
    private partial void LogAbsoluteTranslation(Uri result);

    [LoggerMessage(
        LogLevel.Trace,
        Message = "File name destination: {FileName}")]
    private partial void LogFileNameDestiantion(string fileName);
}
