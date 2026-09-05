using Sylvercode.SiteExtractor.Store;

namespace Sylvercode.SiteExtractor.Tests.Mocks;

/// <summary>Mock <see cref="IDataStore"/> recording write calls for assertion in unit tests.</summary>
public class DataStoreMock(MockCallTracker tracker) : IDataStore
{
    /// <summary>Gets the base output URI used by this mock store.</summary>
    public Uri BaseUri { get; } = new("test://mock-output");

    /// <summary>Records the <see cref="GetStream"/> call via the tracker and returns an empty <see cref="MemoryStream"/>.</summary>
    /// <param name="uri">The URI of the resource whose stream is requested.</param>
    /// <returns>A new empty <see cref="MemoryStream"/>.</returns>
    public Stream GetStream(Uri uri)
    {
        tracker.TrackCall(this, nameof(GetStream), [uri]);
        return new MemoryStream();
    }

    /// <summary>Records the <see cref="GetStreamWriter"/> call via the tracker and returns a writer backed by an empty <see cref="MemoryStream"/>.</summary>
    /// <param name="uri">The URI of the resource to write.</param>
    /// <returns>A new <see cref="StreamWriter"/> over an empty <see cref="MemoryStream"/>.</returns>
    public StreamWriter GetStreamWriter(Uri uri)
    {
        tracker.TrackCall(this, nameof(GetStreamWriter), [uri]);
        return new StreamWriter(new MemoryStream());
    }
}
