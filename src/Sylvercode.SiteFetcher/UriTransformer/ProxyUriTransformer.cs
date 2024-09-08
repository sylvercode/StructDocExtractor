
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Sylvercode.SiteFetcher.UriTransformer;

public partial class ProxyUriTransformer(Uri SourceBase, Uri ProxyBase, ProxyUriTransformer.Options options = new(), ILogger<ProxyUriTransformer>? logger = null) : IUriTransformer
{
    public struct Options
    {
        public bool OtherBaseAsError { get; set; }
    }

    private readonly ILogger<ProxyUriTransformer> _logger = logger ?? NullLogger<ProxyUriTransformer>.Instance;

    public Uri Transform(Uri uri)
    {
        if (!SourceBase.IsBaseOf(uri))
        {
            LogInvalidBase(options.OtherBaseAsError ? LogLevel.Error : LogLevel.Debug, uri);
            if (options.OtherBaseAsError)
                throw new ArgumentException("Invalid base URI", nameof(uri));
            return uri;
        }

        Uri relative = SourceBase.MakeRelativeUri(uri);
        Uri proxyUri = new(ProxyBase, relative);

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
