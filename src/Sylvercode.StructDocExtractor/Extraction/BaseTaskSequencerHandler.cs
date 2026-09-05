using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Extraction.PreviewProvider;

namespace Sylvercode.StructDocExtractor.Extraction;

/// <summary>Abstract base for <see cref="IExtractorTaskSequencerHandler{TExtractionData, TDataDiscriminator}"/> providing shared factory provider, options, preview, and logging infrastructure.</summary>
/// <typeparam name="TExtractionData">The type of source data element being processed.</typeparam>
/// <typeparam name="TDataDiscriminator">The discriminator type used to select node factories.</typeparam>
/// <param name="defaultNodeFactoryProvider">The factory provider used when no task-scoped override is active.</param>
/// <param name="extractorOption">Options controlling error and exception handling behaviour.</param>
/// <param name="dataPreviewProvider">Optional provider for generating diagnostic previews; falls back to <see cref="ToStringPreviewProvider{T}"/> when null.</param>
/// <param name="loggerFactory">Optional logger factory; falls back to a null factory when omitted.</param>
/// <remarks>Subclasses must implement <see cref="OnProcessTask"/> to perform the actual factory lookup and node creation.</remarks>
public abstract class BaseExtractorTaskSequencerHandler<TExtractionData, TDataDiscriminator>(
    IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> defaultNodeFactoryProvider,
    ExtractorOption extractorOption = default,
    IDataPreviewProvider<TExtractionData>? dataPreviewProvider = null,
    ILoggerFactory? loggerFactory = null)
    : IExtractorTaskSequencerHandler<TExtractionData, TDataDiscriminator>
{
    /// <summary>Gets the configured extractor options.</summary>
    public ExtractorOption ExtractorOption { get; } = extractorOption;
    /// <summary>Gets the default node factory provider used when no task-scoped override is active.</summary>
    public IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> DefaultNodeFactoryProvider { get; } = defaultNodeFactoryProvider;
    /// <summary>Gets the data preview provider used for diagnostic logging.</summary>
    public IDataPreviewProvider<TExtractionData> DataPreviewProvider { get; } = dataPreviewProvider ?? new ToStringPreviewProvider<TExtractionData>();
    /// <summary>Gets the logger factory used to create typed loggers for pipeline components.</summary>
    public ILoggerFactory LoggerFactory { get; } = loggerFactory ?? NullLoggerFactory.Instance;

    /// <summary>Returns a short diagnostic preview string for the given source data item.</summary>
    /// <param name="data">The source data to preview.</param>
    /// <returns>A human-readable string representation of <paramref name="data"/>.</returns>
    public string GetDataPreview(TExtractionData? data) => DataPreviewProvider.GetPreview(data);
    /// <summary>Creates a typed logger for the given category using the configured logger factory.</summary>
    /// <typeparam name="TCategoryName">The category type for the logger.</typeparam>
    /// <returns>A typed <see cref="ILogger{TCategoryName}"/> instance.</returns>
    public ILogger<TCategoryName> CreateLogger<TCategoryName>() => LoggerFactory.CreateLogger<TCategoryName>();

    /// <inheritdoc/>
    public abstract IProcessTaskResult<TExtractionData, TDataDiscriminator> OnProcessTask(TaskContext<TExtractionData, TDataDiscriminator> taskContext);
}
