using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Extraction.PreviewProvider;

namespace Sylvercode.StructDocExtractor.Extraction;

public partial class ExtractorTaskSequencerHandler<TExtractionData, TDataDiscriminator>
    : BaseExtractorTaskSequencerHandler<TExtractionData, TDataDiscriminator>
{
    private readonly ExtractorTaskSequencer<TExtractionData, TDataDiscriminator> _sequencer;

    private readonly IDataDiscriminatorFactory<TExtractionData, TDataDiscriminator>? _dataDiscriminatorFactory;

    private readonly ILogger _logger;

    public ExtractorTaskSequencerHandler(
        IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> defaultNodeFactoryProvider,
        ExtractorOption option = default,
        IDataDiscriminatorFactory<TExtractionData, TDataDiscriminator>? dataDiscriminatorFactory = null,
        IDataPreviewProvider<TExtractionData>? dataPreviewProvider = null,
        ILoggerFactory? loggerFactory = null)
        : base(defaultNodeFactoryProvider, option, dataPreviewProvider, loggerFactory)
    {
        _sequencer = new(this);
        _dataDiscriminatorFactory = dataDiscriminatorFactory;
        _logger = CreateLogger<ExtractorTaskSequencerHandler<TExtractionData, TDataDiscriminator>>();
    }

    public override IProcessTaskResult<TExtractionData, TDataDiscriminator>
        OnProcessTask(TaskContext<TExtractionData, TDataDiscriminator> taskContext)
    {
        LogTraceProcessBegin();

        IStructDocNodeFactoryProvider<TExtractionData, TDataDiscriminator> factoryProvider =
            taskContext.GetNodeFactoryProvider();
        LogFactoryProviderInUse(factoryProvider.DebugName);

        TDataDiscriminator discriminator = GetDataDiscriminator(taskContext);
        LogDataDiscriminatorGot(discriminator?.ToString());

        IStructDocNodeFactory<TExtractionData, TDataDiscriminator>? factory =
            factoryProvider.GetFactoryForStack(taskContext.GetStructDataStack(discriminator));
        LogNoNodeFactoryFound(ExtractorOption.MissingNodeFactoryLogLevel, discriminator?.ToString());
        if (factory is null)
            return ProcessTaskResult.NewErrorOrSkipped<TExtractionData, TDataDiscriminator>(ExtractorOption.MissingNodeFactoryAsError);

        return factory.NewNode(taskContext.ExtractionData);
    }

    public virtual TDataDiscriminator GetDataDiscriminator(TaskContext<TExtractionData, TDataDiscriminator> taskContext)
    {
        if (_dataDiscriminatorFactory is null)
            throw new InvalidOperationException("DataDiscriminatorFactory is not set");

        return _dataDiscriminatorFactory.CreateDataDiscriminator(taskContext.ExtractionData);
    }

    [LoggerMessage(
        Level = LogLevel.Trace,
        Message = "Begin process task.")]
    private partial void LogTraceProcessBegin();

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Factory provider `{FactoryProviderName}` in use.")]
    private partial void LogFactoryProviderInUse(string factoryProviderName);

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Data discriminator `{DataDiscriminator}`.")]
    private partial void LogDataDiscriminatorGot(string? dataDiscriminator);

    [LoggerMessage(
        Message = "No factory found for `{dataDiscriminator}`.")]
    private partial void LogNoNodeFactoryFound(LogLevel level, string? dataDiscriminator);
}
