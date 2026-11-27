using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Sylvercode.SiteExtractor.Observation;

/// <summary>Applies metadata returned by resource processing to its resource.</summary>
public sealed partial class ExtractionMetadataHandler(
    ILogger<ExtractionMetadataHandler>? logger = null) : IExtractionObserver
{
    private readonly ILogger<ExtractionMetadataHandler> _logger =
        logger ?? NullLogger<ExtractionMetadataHandler>.Instance;

    /// <inheritdoc/>
    public void OnNext(ExtractionEvent value)
    {
        if (value is not ResourceProcessed processed || processed.Result.NewMetadata.Count == 0)
            return;

        if (_logger.IsEnabled(LogLevel.Trace))
        {
            foreach (var (name, metadata) in processed.Result.NewMetadata)
            {
                string? metadataValue = metadata.GetStrValue();
                LogNewMetadata(name, metadataValue);
            }
        }

        processed.Resource.Metadata.CopyMetadataFrom(processed.Result.NewMetadata);
    }

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "New metadata: {Name} = {Value}")]
    private partial void LogNewMetadata(string name, string? value);
}
