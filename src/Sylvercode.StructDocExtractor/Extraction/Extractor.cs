using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Extraction.PreviewProvider;

namespace Sylvercode.StructDocExtractor.Extraction;

public class Extractor<TExtractionData, TDataDiscriminator>(
    IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> defaultNodeFactoryProvider,
    ExtractorOption option = default,
    IDataDiscriminatorFactory<TExtractionData, TDataDiscriminator>? dataDiscriminatorFactory = null,
    IDataPreviewProvider<TExtractionData>? dataPreviewProvider = null,
    ILoggerFactory? loggerFactory = null
    )
    : IExtractor<TExtractionData>
{
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
