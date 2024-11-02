using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Sylvercode.SiteExtractor.UriUtils;

public partial class StaticDirUriTranslater(
    string outputPath,
    ILogger<StaticDirUriTranslater>? logger) : IUriTranslater
{
    private readonly ILogger<StaticDirUriTranslater> _logger = logger ?? NullLogger<StaticDirUriTranslater>.Instance;

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
