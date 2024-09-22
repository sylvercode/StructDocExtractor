namespace Sylvercode.SiteExtractor.Resources;

public class Resource
{
    public ResourceState State { get; private set; }

    public Uri SourceUri { get; }

    private Uri? _DestinationUri;

    public Resource(ResourcePullType pullType, Uri uri)
    {
        State = new(pullType, ResourcePullState.Pending);
        SourceUri = uri;
    }

    public Uri GetDestinationFor(Uri uri)
    {
        if (State.PullType is ResourcePullType.NoPull)
            return SourceUri;

        // TODO: Implement this method
        throw new NotImplementedException();
    }

    public void MarkAsPulling()
    {
        if (!State.IsPullNeeded)
            throw new InvalidOperationException("Cannot mark a resource as pulling if it doesn't need to be pulled.");

        if (!State.IsPulling)
            throw new InvalidOperationException("Cannot mark a resource as pulling if it has already pulling.");

        State = new ResourceState(State.PullType, ResourcePullState.Pulling);
    }

    public void MarkAsPulled(Uri destinationUri)
    {
        if (State.PullType is ResourcePullType.NoPull)
            throw new InvalidOperationException("Cannot mark a resource as pulled if it doesn't need to be pulled.");

        if (!string.IsNullOrEmpty(destinationUri.Fragment))
            throw new InvalidOperationException("The destination URI cannot have a fragment.");

        _DestinationUri = destinationUri;
        State = new ResourceState(State.PullType, ResourcePullState.Pulled);
    }
}
