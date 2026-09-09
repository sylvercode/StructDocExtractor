using Microsoft.Extensions.Logging;
using Sylvercode.StructDocExtractor.Extraction.Factory;
using Sylvercode.StructDocExtractor.Extraction.PreviewProvider;

namespace Sylvercode.StructDocExtractor.Extraction;

/// <summary>Default implementation of <see cref="BaseExtractorTaskSequencerHandler{TExtractionData, TDataDiscriminator}"/> that resolves the correct node factory via the data discriminator and delegates to it for node creation.</summary>
/// <typeparam name="TExtractionData">The type of source data element being processed.</typeparam>
/// <typeparam name="TDataDiscriminator">The discriminator type used to select node factories.</typeparam>
public partial class ExtractorTaskSequencerHandler<TExtractionData, TDataDiscriminator>
    : BaseExtractorTaskSequencerHandler<TExtractionData, TDataDiscriminator>
{
    private readonly ExtractorTaskSequencer<TExtractionData, TDataDiscriminator> _sequencer;

    private readonly IDataDiscriminatorFactory<TExtractionData, TDataDiscriminator>? _dataDiscriminatorFactory;

    private readonly ILogger _logger;

    /// <summary>Initializes a new instance of <see cref="ExtractorTaskSequencerHandler{TExtractionData, TDataDiscriminator}"/>.</summary>
    /// <param name="defaultNodeFactoryProvider">The factory provider used when no task-scoped override is active.</param>
    /// <param name="option">Extractor options; uses defaults when omitted.</param>
    /// <param name="dataDiscriminatorFactory">Optional discriminator factory; required unless <see cref="GetDataDiscriminator"/> is overridden in a subclass.</param>
    /// <param name="dataPreviewProvider">Optional preview provider for logging.</param>
    /// <param name="loggerFactory">Optional logger factory.</param>
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

    /// <inheritdoc/>
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
        if (factory is null)
        {
            LogNoNodeFactoryFound(ExtractorOption.MissingNodeFactoryLogLevel, discriminator?.ToString());
            return ProcessTaskResult.NewErrorOrSkipped<TExtractionData, TDataDiscriminator>(ExtractorOption.MissingNodeFactoryAsError);
        }
        else
            LogNodeFactoryFound(factory.GetType().Name);

        return factory.NewNode(discriminator, taskContext.ExtractionData);
    }

    /// <summary>Computes and returns the data discriminator for the current task using the injected <see cref="IDataDiscriminatorFactory{TExtractionData, TDataDiscriminator}"/>.</summary>
    /// <param name="taskContext">The typed context for the current extraction task.</param>
    /// <returns>The discriminator value derived from the task's source data.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no <see cref="IDataDiscriminatorFactory{TExtractionData, TDataDiscriminator}"/> was provided at construction.</exception>
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

    [LoggerMessage(
        Level = LogLevel.Debug,
        Message = "Factory `{FactoryName}` found.")]
    private partial void LogNodeFactoryFound(string factoryName);
}
