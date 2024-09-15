using Microsoft.Extensions.Logging;
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
    : IObservable<ExtractionTask>
    where TExtractionData : notnull
{
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
        extractorTaskSequencer.TaskResultSet += OnTaskResult;

        extractorTaskSequencer.AddTask(data);
        ExtractionResult result = extractorTaskSequencer.ProcessTasks();
        OnCompleted();
        return result;
    }

    private void OnTaskResult(object? sender, EventArgs e)
    {
        if (sender is not ExtractionTask task)
            return;

        foreach (var observer in observers)
            observer.OnNext(task);
    }

    private void OnCompleted()
    {
        foreach (var observer in observers)
            observer.OnCompleted();
    }

    #region IObservable
    private readonly List<IObserver<ExtractionTask>> observers = [];
    public IDisposable Subscribe(IObserver<ExtractionTask> observer)
    {
        if (!observers.Contains(observer))
            observers.Add(observer);
        return new Unsubscriber(observers, observer);
    }

    private sealed class Unsubscriber(List<IObserver<ExtractionTask>> observers, IObserver<ExtractionTask> observer) : IDisposable
    {
        private readonly List<IObserver<ExtractionTask>> _observers = observers;
        private readonly IObserver<ExtractionTask> _observer = observer;

        public void Dispose()
        {
            if (_observer != null && _observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
    #endregion

}
