using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Extraction.Factory;

namespace Sylvercode.StructDocExtractor.Extraction;

public interface IExtractorTaskSequencerHandler<TExtractionData, TDataDiscriminator>
{
    ExtractorOption ExtractorOption { get; }
    string GetDataPreview(TExtractionData? data);
    IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> DefaultNodeFactoryProvider { get; }
    ILogger<TCategoryName> CreateLogger<TCategoryName>();

    IProcessTaskResult<TExtractionData, TDataDiscriminator>
        OnProcessTask(TaskContext<TExtractionData, TDataDiscriminator> taskContext);
}
