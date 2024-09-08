using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Extraction.PreviewProvider;

namespace Sylvercode.StructDocExtractor.Extraction;

public class Extractor<TExtractionData, TDataDiscriminator>(
    IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> defaultNodeFactoryProvider,
    ExtractorOption option = default,
    IChildrenTaskInfoFactory? childrenTaskInfoFactory = null,
    IDataDiscriminatorFactory<TExtractionData, TDataDiscriminator>? dataDiscriminatorFactory = null,
    IDataPreviewProvider<TExtractionData>? dataPreviewProvider = null,
    ILoggerFactory? loggerFactory = null
    )
    where TExtractionData : notnull
{
    public event EventHandler? TaskResultSet;

    public ExtractionResult Extract(TExtractionData data)
    {
        ExtractorTaskSequencerHandler<TExtractionData, TDataDiscriminator> handler = new(
            defaultNodeFactoryProvider,
            option,
            childrenTaskInfoFactory,
            dataDiscriminatorFactory,
            dataPreviewProvider,
            loggerFactory
            );
        ExtractorTaskSequencer<TExtractionData, TDataDiscriminator> extractorTaskSequencer = new(handler);

        if (TaskResultSet is not null)
            extractorTaskSequencer.TaskResultSet += TaskResultSet;

        extractorTaskSequencer.AddTask(data);
        return extractorTaskSequencer.ProcessTasks();
    }

}
