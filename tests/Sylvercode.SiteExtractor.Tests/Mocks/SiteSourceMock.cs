using Sylvercode.SiteExtractor.Sources;

namespace Sylvercode.SiteExtractor.Tests.Mocks;

public class SiteSourceMock(MockCallTracker tracker, bool returnsNull = false) : ISiteSource<Uri?>
{
    public Uri BaseUri { get; } = new("test://mock-input");

    public bool CanGetFrom(Uri uri) => true;

    public bool DataExists(Uri uri) => true;

    public Uri? GetData(Uri uri)
    {
        tracker.TrackCall(this, nameof(GetData), [uri]);
        return returnsNull ? null : uri;
    }
}
