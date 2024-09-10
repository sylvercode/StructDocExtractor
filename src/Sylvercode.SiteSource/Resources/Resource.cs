using Sylvercode.SiteSource.UriTransformer;

namespace Sylvercode.SiteSource.Resources;

public class Resource
{
    public ResourceState State { get; private set; }

    public Uri SourceUri { get; }

    private readonly HashSet<string> _fragments;
    public IReadOnlySet<string> Fragments => _fragments;

    private bool _hasFragments;

    private Uri? _DestinationUri;

    public Resource(ResourcePullType pullType, Uri uri, string fragment = "")
    {
        if (pullType != ResourcePullType.NoPull && !string.IsNullOrEmpty(fragment) && !fragment.StartsWith('#'))
            throw new ArgumentException("The fragment must start with a #.", nameof(fragment));

        State = new(pullType);
        SourceUri = uri;
        _fragments = [fragment];
        _hasFragments = !string.IsNullOrEmpty(fragment);
    }

    public Uri DestinationUri
    {
        get
        {
            if (State.PullType is ResourcePullType.NoPull)
                return SourceUri;

            if (_DestinationUri is null)
                throw new InvalidOperationException("The resource has not been pulled yet.");

            if (_hasFragments)
                throw new InvalidOperationException("The resource has fragments.");

            return _DestinationUri;
        }
    }

    public Uri GetDestinationFor(Uri uri)
    {
        if (State.PullType is ResourcePullType.NoPull)
            return SourceUri;

        if (!Fragments.Contains(uri.Fragment))
            throw new InvalidOperationException("The fragment is not part of the resource.");

        if (_DestinationUri is null)
            throw new InvalidOperationException("The resource has not been pulled yet.");

        return new Uri(_DestinationUri, uri.Fragment);
    }

    public void AddFragment(string fragment)
    {
        _fragments.Add(fragment);
        _hasFragments = _hasFragments || Fragments.Count > 1 || !string.IsNullOrEmpty(fragment);
    }

    public void MarkAsPulled(Uri destinationUri)
    {
        if (State.PullType is ResourcePullType.NoPull)
            throw new InvalidOperationException("Cannot mark a resource as pulled if it doesn't need to be pulled.");

        if (!string.IsNullOrEmpty(destinationUri.Fragment))
            throw new InvalidOperationException("The destination URI cannot have a fragment.");

        _DestinationUri = destinationUri;
        State = new ResourceState(State.PullType, true);
    }
}
