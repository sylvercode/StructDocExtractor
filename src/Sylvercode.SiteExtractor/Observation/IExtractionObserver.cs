namespace Sylvercode.SiteExtractor.Observation;

/// <summary>Receives the synchronous event stream emitted during site extraction.</summary>
public interface IExtractionObserver : IObserver<ExtractionEvent>
{
    /// <inheritdoc/>
    void IObserver<ExtractionEvent>.OnCompleted()
    {
    }

    /// <inheritdoc/>
    void IObserver<ExtractionEvent>.OnError(Exception error)
    {
    }
}
