using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Extraction.Factory;

namespace Sylvercode.StructDocExtractor.Extraction;

/// <summary>Defines the contract for a handler that processes individual extraction tasks within a sequenced extractor pipeline stage.</summary>
/// <typeparam name="TExtractionData">The type of source data being processed.</typeparam>
/// <typeparam name="TDataDiscriminator">The type of the data discriminator used to route factory selection.</typeparam>
public interface IExtractorTaskSequencerHandler<TExtractionData, TDataDiscriminator>
{
    /// <summary>Gets the configuration options governing this extractor's behaviour.</summary>
    ExtractorOption ExtractorOption { get; }
    /// <summary>Returns a short human-readable preview of <paramref name="data"/> for logging and diagnostics.</summary>
    /// <param name="data">The source data item to summarize; may be <see langword="null"/>.</param>
    /// <returns>A preview string suitable for log messages.</returns>
    string GetDataPreview(TExtractionData? data);
    /// <summary>Gets the default factory provider used when no context-specific provider is resolved for the task.</summary>
    IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> DefaultNodeFactoryProvider { get; }
    /// <summary>Creates a typed logger for the specified category.</summary>
    /// <typeparam name="TCategoryName">The category type used to name the logger.</typeparam>
    /// <returns>An <see cref="ILogger{TCategoryName}"/> for structured logging.</returns>
    ILogger<TCategoryName> CreateLogger<TCategoryName>();

    /// <summary>Processes a single extraction task and returns its typed result.</summary>
    /// <param name="taskContext">The context describing the current extraction task.</param>
    /// <returns>The typed <see cref="IProcessTaskResult{TExtractionData, TDataDiscriminator}"/> produced by processing the task.</returns>
    IProcessTaskResult<TExtractionData, TDataDiscriminator>
        OnProcessTask(TaskContext<TExtractionData, TDataDiscriminator> taskContext);
}
