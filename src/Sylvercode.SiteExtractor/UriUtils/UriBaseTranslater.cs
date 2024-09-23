
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Sylvercode.SiteExtractor.UriUtils;

public partial class UriBaseTranslater(
    UriBaseTranslater.Options options = new(),
    ILogger<UriBaseTranslater>? logger = null) : IUriTranslater
{
    public struct Options
    {
        public bool OtherBaseAsError { get; set; }
    }

    private readonly ILogger<UriBaseTranslater> _logger = logger ?? NullLogger<UriBaseTranslater>.Instance;


    public Uri Translate(Uri uri, Uri storeBaseUri, Uri? sourceBaseUri = null)
    {
        sourceBaseUri ??= uri;

        if (!sourceBaseUri.IsBaseOf(uri))
        {
            LogInvalidBase(options.OtherBaseAsError ? LogLevel.Error : LogLevel.Debug, uri);
            if (options.OtherBaseAsError)
                throw new ArgumentException("Invalid base URI", nameof(uri));
            return uri;
        }

        Uri relative = sourceBaseUri.MakeRelativeUri(uri);
        Uri proxyUri = new(storeBaseUri, relative);

        LogProxyUri(uri, proxyUri);
        return proxyUri;
    }

    [LoggerMessage(
        Message = "Invalid base URI: {Uri}")]
    private partial void LogInvalidBase(LogLevel level, Uri uri);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Proxy URI: {uri} -> {proxyUri}")]
    private partial void LogProxyUri(Uri uri, Uri proxyUri);
}
