using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Extraction.PreviewProvider;

namespace Sylvercode.StructDocExtractor.Extraction;

/// <summary>Main implementation of <see cref="IExtractor{TExtractionData}"/> that drives the recursive extraction pipeline for a single source root.</summary>
/// <typeparam name="TExtractionData">The type of source data element to extract.</typeparam>
/// <typeparam name="TDataDiscriminator">The discriminator type used to select node factories.</typeparam>
/// <param name="defaultNodeFactoryProvider">The factory provider used to resolve node factories when no task-scoped override is active.</param>
/// <param name="option">Options controlling error handling behaviour; uses defaults when omitted.</param>
/// <param name="dataDiscriminatorFactory">Optional factory for computing data discriminators; required unless the handler overrides <c>GetDataDiscriminator</c>.</param>
/// <param name="dataPreviewProvider">Optional provider for generating log-friendly data previews.</param>
/// <param name="loggerFactory">Optional logger factory for structured logging within the pipeline.</param>
/// <remarks>Delegates task sequencing to <see cref="ExtractorTaskSequencer{TExtractionData, TDataDiscriminator}"/> and task processing to <see cref="ExtractorTaskSequencerHandler{TExtractionData, TDataDiscriminator}"/>.</remarks>
public class Extractor<TExtractionData, TDataDiscriminator>(
    IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> defaultNodeFactoryProvider,
    ExtractorOption option = default,
    IDataDiscriminatorFactory<TExtractionData, TDataDiscriminator>? dataDiscriminatorFactory = null,
    IDataPreviewProvider<TExtractionData>? dataPreviewProvider = null,
    ILoggerFactory? loggerFactory = null
    )
    : IExtractor<TExtractionData>
{
    /// <inheritdoc/>
    public ExtractionResult Extract([DisallowNull] TExtractionData data, IObserver<ExtractionTask>? observer = null)
    {
        ExtractorTaskSequencerHandler<TExtractionData, TDataDiscriminator> handler = new(
            defaultNodeFactoryProvider,
            option,
            dataDiscriminatorFactory,
            dataPreviewProvider,
            loggerFactory
            );
        ExtractorTaskSequencer<TExtractionData, TDataDiscriminator> extractorTaskSequencer = new(handler);
        if (observer is not null)
            extractorTaskSequencer.Subscribe(observer);

        extractorTaskSequencer.AddTask(data);
        ExtractionResult result = extractorTaskSequencer.ProcessTasks();
        return result;
    }
}
