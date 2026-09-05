using Sylvercode.SiteExtractor.Sources;

namespace Sylvercode.SiteExtractor.Tests.Mocks;

/// <summary>Mock <see cref="ISiteSource{TData}"/> serving predefined document strings by URI.</summary>
public class SiteSourceMock(MockCallTracker tracker, bool returnsNull = false) : ISiteSource<Uri?>
{
    /// <summary>Gets the base input URI used by this mock source.</summary>
    public Uri BaseUri { get; } = new("test://mock-input");

    /// <inheritdoc/>
    public bool CanGetFrom(Uri uri) => true;

    /// <inheritdoc/>
    public bool DataExists(Uri uri) => true;

    /// <summary>Records the call via the tracker and returns the input <paramref name="uri"/> unchanged, or <see langword="null"/> when configured to do so.</summary>
    /// <param name="uri">The URI for which data is requested.</param>
    /// <returns>The same <paramref name="uri"/> if <c>returnsNull</c> is <see langword="false"/>; otherwise <see langword="null"/>.</returns>
    public Uri? GetData(Uri uri)
    {
        tracker.TrackCall(this, nameof(GetData), [uri]);
        return returnsNull ? null : uri;
    }
}
