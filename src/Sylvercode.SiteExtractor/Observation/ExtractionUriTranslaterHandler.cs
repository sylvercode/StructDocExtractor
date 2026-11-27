using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Sylvercode.SiteExtractor.Observation;

/// <summary>Applies URI translators returned by resource processing to their resource.</summary>
/// <param name="logger">Optional logger for translator-assignment diagnostics.</param>
public sealed partial class ExtractionUriTranslaterHandler(
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
            processed.Result.ResourceUriTranslaterToSet);
    }

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Setting UriTranslater: {UriTranslater}")]
    private partial void LogUriTranslater(string uriTranslater);
}
