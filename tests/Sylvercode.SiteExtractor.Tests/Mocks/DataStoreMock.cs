using Sylvercode.SiteExtractor.Store;

namespace Sylvercode.SiteExtractor.Tests.Mocks;

public class DataStoreMock(MockCallTracker tracker) : IDataStore
{
    public Uri BaseUri { get; } = new("test://mock-output");

    public Stream GetStream(Uri uri)
    {
        tracker.TrackCall(this, nameof(GetStream), [uri]);
        return new MemoryStream();
    }

    public StreamWriter GetStreamWriter(Uri uri)
    {
        tracker.TrackCall(this, nameof(GetStreamWriter), [uri]);
        return new StreamWriter(new MemoryStream());
    }
}
