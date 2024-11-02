using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Sylvercode.SiteExtractor.UriUtils;

public partial class RelativeUriTranslater(
    Uri SourceBaseUri,
    string outputPath,
    ILogger<RelativeUriTranslater>? logger) : IUriTranslater
{
    private readonly ILogger<RelativeUriTranslater> _logger = logger ?? NullLogger<RelativeUriTranslater>.Instance;

    private readonly Uri _outBaseDir = new(outputPath, UriKind.Relative);

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
