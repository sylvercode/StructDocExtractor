using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.SiteExtractor.Resources;
using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Observation;

/// <summary>Applies URI translators returned by resource processing to their resource.</summary>
/// <param name="queryIdentityFilter">The policy used to canonicalize query identity before translation.</param>
/// <param name="logger">Optional logger for translator-assignment diagnostics.</param>
public sealed partial class ExtractionUriTranslaterHandler(
    IUriQueryIdentityFilter queryIdentityFilter,
    ILogger<ExtractionUriTranslaterHandler>? logger = null) : IExtractionObserver
{
    private readonly ILogger<ExtractionUriTranslaterHandler> _logger =
        logger ?? NullLogger<ExtractionUriTranslaterHandler>.Instance;

    /// <inheritdoc/>
    public void OnNext(ExtractionEvent value)
    {
        if (value is not ResourceProcessed processed
            || processed.Result.ResourceUriTranslaterToSet is null)
        {
            return;
        }

        if (_logger.IsEnabled(LogLevel.Debug))
        {
            LogUriTranslater(processed.Result.ResourceUriTranslaterToSet.GetType().Name);
        }

        processed.Resource.SetUriTranslater(
            new QueryFilteredResourceUriTranslater(
                processed.Result.ResourceUriTranslaterToSet,
                queryIdentityFilter));
    }

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Setting UriTranslater: {UriTranslater}")]
    private partial void LogUriTranslater(string uriTranslater);
}
