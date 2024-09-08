using Sylvercode.SiteFetcher.UriTransformer;

namespace Sylvercode.SiteFetcher.Resources;

public class Resource(Uri uri, ResourcePullType pullType, IUriTransformer? sourceUriTransformer = null)
{
    public Uri Uri { get; } = uri;

    public ResourceState State { get; private set; } = new(pullType);

    public IUriTransformer SourceUriTransformer { get; } = sourceUriTransformer ?? NoopUriTransformer.Default;

    public Uri SourceUri => SourceUriTransformer.Transform(Uri);

    private Uri? _DestinationUri;
    public Uri DestinationUri
    {
        get
        {
            if (State.PullType is ResourcePullType.NoPull)
                return Uri;

            if (_DestinationUri is null)
                throw new InvalidOperationException("The resource has not been pulled yet.");

            return _DestinationUri;
        }
        private set
        {
            _DestinationUri = value;
        }
    }
    public void MarkAsPulled(Uri destinationUri)
    {
        if (State.PullType is ResourcePullType.NoPull)
            throw new InvalidOperationException("Cannot mark a resource as pulled if it doesn't need to be pulled.");

        _DestinationUri = destinationUri;
        State = new ResourceState(State.PullType, true);
    }
}
