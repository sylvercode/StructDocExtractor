namespace Sylvercode.SiteFetcher.Extractor;

public class Resource(Uri uri, ResourcePullType pullType)
{
    public Uri Uri { get; } = uri;

    public ResourceState State { get; private set; } = new(pullType);

    private Uri? _outUri;
    public Uri OutUri
    {
        get
        {
            if (State.PullType is ResourcePullType.NoPull)
                return Uri;

            if (_outUri is null)
                throw new InvalidOperationException("The resource has not been pulled yet.");

            return _outUri;
        }
        private set
        {
            _outUri = value;
        }
    }
    public void MarkAsPulled(Uri newUri)
    {
        if (State.PullType is ResourcePullType.NoPull)
            throw new InvalidOperationException("Cannot mark a resource as pulled if it doesn't need to be pulled.");

        _outUri = newUri;
        State = new ResourceState(State.PullType, true);
    }
}
