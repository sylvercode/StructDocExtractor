using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Sylvercode.SiteExtractor.UriUtils;

namespace Sylvercode.SiteExtractor.Resources;

public class ResourceRepository() : IReadOnlyResourceRepository
{
    private readonly Dictionary<Uri, Resource> _resources = [];

    public (Resource resource, bool isNew) Add(Uri uri, bool isPullable = true)
    {
        (Uri uriWithoutFragment, _) = uri.GetUriAndFragment();
        if (_resources.TryGetValue(uriWithoutFragment, out Resource? resource))
        {
            if (resource.State.IsPullable != isPullable)
                throw new InvalidOperationException("The resource already exists with a different pull type.");
            return (resource, false);
        }

        var result = (resource: new Resource(uriWithoutFragment, isPullable), isNew: true);
        _resources[uriWithoutFragment] = result.resource;

        return result;
    }

    public ResourceRepository(IEnumerable<string> uris) : this()
    {
        foreach (var uriString in uris)
            Add(new Uri(uriString));
    }

    public Resource this[Uri key]
    {
        get
        {
            (Uri uriWithoutFragment, _) = key.GetUriAndFragment();
            var result = _resources[uriWithoutFragment];
            return result;
        }
    }

    public IReadOnlyDictionary<Uri, Resource> AsDictionary() => _resources.AsReadOnly();

    public IEnumerable<Uri> UriResources => _resources.Keys;

    public int Count => _resources.Count;

    public bool ContainsResourceForUri(Uri key)
    {
        (Uri uriWithoutFragment, _) = key.GetUriAndFragment();
        return _resources.ContainsKey(uriWithoutFragment);
    }

    public bool TryGetResourceForUri(Uri key, [MaybeNullWhen(false)] out Resource value)
        => _resources.TryGetValue(key, out value);

    #region IEnumerable<Resource> implementation
    public IEnumerator<Resource> GetEnumerator()
        => _resources.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => _resources.Values.GetEnumerator();
    #endregion
}
