using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Extraction.PreviewProvider;

namespace Sylvercode.StructDocExtractor.Extraction;

public abstract class BaseExtractorTaskSequencerHandler<TExtractionData, TDataDiscriminator>(
    IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> defaultNodeFactoryProvider,
    ExtractorOption extractorOption = default,
    IChildrenTaskInfoFactory? childrenTaskInfoFactory = null,
    IDataPreviewProvider<TExtractionData>? dataPreviewProvider = null,
    ILoggerFactory? loggerFactory = null)
    : IExtractorTaskSequencerHandler<TExtractionData, TDataDiscriminator>
{
    public ExtractorOption ExtractorOption { get; } = extractorOption;
    public IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> DefaultNodeFactoryProvider { get; } = defaultNodeFactoryProvider;
    public IChildrenTaskInfoFactory ChildrenTaskInfoFactory { get; } = childrenTaskInfoFactory ?? Factory.ChildrenTaskInfoFactory.Default;
    public IDataPreviewProvider<TExtractionData> DataPreviewProvider { get; } = dataPreviewProvider ?? new ToStringPreviewProvider<TExtractionData>();
    public ILoggerFactory LoggerFactory { get; } = loggerFactory ?? NullLoggerFactory.Instance;

    public string GetDataPreview(TExtractionData? data) => DataPreviewProvider.GetPreview(data);
    public ILogger<TCategoryName> CreateLogger<TCategoryName>() => LoggerFactory.CreateLogger<TCategoryName>();

    public abstract IProcessTaskResult<TExtractionData, TDataDiscriminator> OnProcessTask(TaskContext<TExtractionData, TDataDiscriminator> taskContext);
}
